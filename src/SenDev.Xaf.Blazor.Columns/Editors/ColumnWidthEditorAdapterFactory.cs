using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;
using DevExpress.ExpressApp.Model;

namespace SenDev.Xaf.Blazor.Columns.Editors;

internal static class ColumnWidthEditorAdapterFactory
{
    private static readonly ConcurrentDictionary<Type, Type?> GridAdapters = new();
    private static readonly ConcurrentDictionary<Type, Type?> TreeAdapters = new();
    private static readonly AssemblyBuilder DynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(
        new AssemblyName("SenDev.Xaf.Blazor.Columns.DynamicEditors"),
        AssemblyBuilderAccess.Run);
    private static readonly ModuleBuilder DynamicModule = DynamicAssembly.DefineDynamicModule("SenDev.Xaf.Blazor.Columns.DynamicEditors");
    private static readonly object SyncRoot = new();
    private static int typeCounter;

    public static Type? GetGridAdapterType(Type editorType)
        => GridAdapters.GetOrAdd(editorType, static type => CreateAdapterType(type, ColumnWrapperKind.Grid));

    public static Type? GetTreeAdapterType(Type editorType)
        => TreeAdapters.GetOrAdd(editorType, static type => CreateAdapterType(type, ColumnWrapperKind.Tree));

    public static bool IsGeneratedAdapterType(Type editorType)
        => editorType.Assembly == DynamicAssembly;

    private static Type? CreateAdapterType(Type editorType, ColumnWrapperKind columnWrapperKind)
    {
        if (editorType.IsSealed || editorType.IsAbstract)
        {
            return null;
        }

        var constructor = editorType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(IModelListView)],
            modifiers: null);
        if (constructor is null)
        {
            return null;
        }

        var createColumnWrapperMethod = editorType.GetMethod(
            "CreateColumnWrapper",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(DxDataColumnBaseModel)],
            modifiers: null);
        if (createColumnWrapperMethod is null || !createColumnWrapperMethod.IsVirtual || createColumnWrapperMethod.IsFinal)
        {
            return null;
        }

        lock (SyncRoot)
        {
            var typeBuilder = DynamicModule.DefineType(
                GetTypeName(editorType, columnWrapperKind),
                TypeAttributes.Public | TypeAttributes.Class,
                editorType,
                [typeof(ISupportsColumnWidthMode)]);

            DefineConstructor(typeBuilder, constructor);
            DefineCreateColumnWrapperOverride(typeBuilder, createColumnWrapperMethod, columnWrapperKind);
            DefineInterfaceProperty(typeBuilder, nameof(ISupportsColumnWidthMode.ListViewModel), nameof(ColumnWidthEditorSupport.GetListViewModel));
            DefineInterfaceProperty(typeBuilder, nameof(ISupportsColumnWidthMode.ColumnsModel), nameof(ColumnWidthEditorSupport.GetColumnsModel));
            DefineInterfaceProperty(typeBuilder, nameof(ISupportsColumnWidthMode.ApplicationOptionsModel), nameof(ColumnWidthEditorSupport.GetApplicationOptionsModel));

            return typeBuilder.CreateTypeInfo()?.AsType();
        }
    }

    private static void DefineConstructor(TypeBuilder typeBuilder, ConstructorInfo baseConstructor)
    {
        var constructorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            [typeof(IModelListView)]);

        var il = constructorBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, baseConstructor);
        il.Emit(OpCodes.Ret);
    }

    private static void DefineCreateColumnWrapperOverride(TypeBuilder typeBuilder, MethodInfo baseMethod, ColumnWrapperKind columnWrapperKind)
    {
        var methodBuilder = typeBuilder.DefineMethod(
            baseMethod.Name,
            MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig,
            typeof(DxGridColumnWrapperBase),
            [typeof(DxDataColumnBaseModel)]);

        var helperMethod = typeof(ColumnWidthEditorSupport).GetMethod(
            columnWrapperKind == ColumnWrapperKind.Grid
                ? nameof(ColumnWidthEditorSupport.CreateGridColumnWrapper)
                : nameof(ColumnWidthEditorSupport.CreateTreeListColumnWrapper))!;

        var il = methodBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, helperMethod);
        il.Emit(OpCodes.Ret);

        typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
    }

    private static void DefineInterfaceProperty(TypeBuilder typeBuilder, string propertyName, string helperMethodName)
    {
        var propertyBuilder = typeBuilder.DefineProperty(
            propertyName,
            PropertyAttributes.None,
            typeof(IModelBlazorColumnWidthMode),
            Type.EmptyTypes);

        var methodBuilder = typeBuilder.DefineMethod(
            $"get_{propertyName}",
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
            typeof(IModelBlazorColumnWidthMode),
            Type.EmptyTypes);

        var helperMethod = typeof(ColumnWidthEditorSupport).GetMethod(helperMethodName)!;
        var il = methodBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, helperMethod);
        il.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(methodBuilder);
        typeBuilder.DefineMethodOverride(methodBuilder, typeof(ISupportsColumnWidthMode).GetProperty(propertyName)!.GetMethod!);
    }

    private static string GetTypeName(Type editorType, ColumnWrapperKind columnWrapperKind)
    {
        var editorName = editorType.FullName ?? editorType.Name;
        var safeEditorName = editorName.Replace('.', '_').Replace('+', '_').Replace('`', '_');
        var suffix = columnWrapperKind == ColumnWrapperKind.Grid ? "Grid" : "Tree";
        var index = Interlocked.Increment(ref typeCounter);
        return $"SenDev.Xaf.Blazor.Columns.Dynamic.{safeEditorName}_{suffix}ColumnWidthAdapter_{index}";
    }

    private enum ColumnWrapperKind
    {
        Grid,
        Tree
    }
}

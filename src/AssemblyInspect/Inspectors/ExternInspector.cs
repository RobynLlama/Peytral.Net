using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace AssemblyInspect.Inspectors;

public class ExternInspector
{

  protected AssemblyDefinition assembly;

  protected IEnumerable<MethodReference> MethodRefs;
  protected IEnumerable<TypeDefinition> TypeDefs;
  protected IEnumerable<MethodDefinition> MethodDefs;
  protected IEnumerable<MethodDefinition>? PInvokes;

  public ExternInspector(string assemblyPath)
  {
    assembly = AssemblyDefinition.ReadAssembly(assemblyPath);

    MethodRefs = assembly.Modules
      .SelectMany(module => module.GetMemberReferences())
      .OfType<MethodReference>();

    TypeDefs = assembly.Modules
      .SelectMany(module => module.GetAllTypes());

    MethodDefs = TypeDefs
      .SelectMany(type => type.GetMethods());
  }

  public IEnumerable<MethodReference> GetExtMemberRefs() =>
    MethodRefs;

  public IEnumerable<MethodDefinition> GetPInvokes()
  {
    PInvokes ??= MethodDefs
      .Where(method => (method.Attributes & MethodAttributes.PInvokeImpl) != 0);

    //string dllName = method.PInvokeInfo.Module.Name;
    //string entryPoint = method.PInvokeInfo.EntryPoint;

    return PInvokes;
  }
}


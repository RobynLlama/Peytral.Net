using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace AssemblyInspector.Inspectors;

public class Inspector
{

  protected AssemblyDefinition assembly;

  protected IEnumerable<MethodReference> MethodRefs;
  protected IEnumerable<TypeDefinition> TypeDefs;
  protected IEnumerable<MethodDefinition> MethodDefs;

  public Inspector(string assemblyPath)
  {
    Console.WriteLine($"Attempting to read: {Path.GetFileName(assemblyPath)}");

    assembly = AssemblyDefinition.ReadAssembly(assemblyPath);

    Console.WriteLine($"== Assembly Loaded ==\n");
    Console.WriteLine($"  Name: {assembly.Name}\n");

    //When LINQ goes too far
    /*
      Todo:
      Worth making these init lazily as needed
    */

    MethodRefs = assembly.Modules
      .SelectMany(module => module.GetMemberReferences())
      .OfType<MethodReference>();

    TypeDefs = assembly.Modules
      .SelectMany(module => module.GetAllTypes());

    MethodDefs = TypeDefs
      .SelectMany(type => type.GetMethods());
  }

  public void OutputExtMemberRefs()
  {
    Console.WriteLine($"== External Method References ==");
    /*
      This is where all Type, Method and Declaring Type checking would be done
      Ideally this whole method is rewritten to return a boolean and take in
      a list of suspicious types or methods and then if it returns true
      we go in with the deeper inspection of each member's IL to see why
      it uses the flagged method or something, dunno
    */
    /*
      Note that this catches constructors for types so can see if they're
      creating Process objects to launch, for example.
    */
    foreach (var methodRef in MethodRefs)
      Console.WriteLine($"  {methodRef.DeclaringType.FullName}::{methodRef.Name}");
  }

  public void OutputExterns()
  {
    Console.WriteLine("== Externs ==");

    //Sort into Methods that have PInvoke flag
    var externs = MethodDefs
      .Where(method => (method.Attributes & MethodAttributes.PInvokeImpl) != 0);

    foreach (var method in externs)
    {
      /*
        This is where we can white/black list by extern DLL and/or
        the method it binds to or just blacklist all externs at all
      */
      string dllName = method.PInvokeInfo.Module.Name;
      string entryPoint = method.PInvokeInfo.EntryPoint;

      Console.WriteLine($"  {method.DeclaringType.FullName}::{method.Name} -> {dllName} ({entryPoint})");
    }
  }

  public void OutputExternalMethodCallLocations()
  {
    Console.WriteLine("== Call Locations ==");

    var declaredMethods = MethodDefs.
      Where(method => method.HasBody);

    foreach (var method in declaredMethods)
    {
      var instructions = method.Body.Instructions;
      foreach (var instr in instructions)
      {
        if (instr.OpCode == OpCodes.Call || instr.OpCode == OpCodes.Callvirt)
          //If the method being called is declared in another assembly
          if (instr.Operand is MethodReference methodRef && !TypeDefs.Contains(methodRef.DeclaringType))
          {
            /*
              This is where filtering external calls or running heuristics on them
              can be done
            */
            Console.WriteLine($"{method.Name} -> {methodRef.DeclaringType.Name}.{methodRef.Name}");
            if (methodRef.DeclaringType.Name.Equals("MethodBase", StringComparison.InvariantCultureIgnoreCase) && methodRef.Name.Equals("Invoke", StringComparison.InvariantCultureIgnoreCase))
            {
              /*
                Really bad example heuristic that just checks the method name
                And then dumps the IL of the method body if it detects a
                MethodBase.Invoke call or call virtual
              */
              Console.WriteLine($"  Warning: MethodBase Invocation detected in {method.FullName} dumping method:");
              foreach (var code in method.Body.Instructions)
              {
                Console.WriteLine($"    {code.Offset} : {code.OpCode.Name}");
                if (code.Operand is not null)
                  Console.WriteLine($"      {code.Operand}");
              }
            }

          }
      }
    }
  }
}


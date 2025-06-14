using System;
using System.Reflection;

/*
  Notice: This will compile but will not actually run
  it should create a signature that the inspector can
  trace to see that Reflection is being used to call
  System.Reflection which is suspicious
*/

var pathToMyThing = "this/is/a/demo";
typeof(Assembly).GetMethod("Lo" + "adFrom").Invoke(null, [pathToMyThing]);

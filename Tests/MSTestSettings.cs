using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.CompilerServices;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
[assembly: InternalsVisibleTo("Tests")]

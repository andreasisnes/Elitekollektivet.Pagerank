[![Build Status](https://dev.azure.com/andreasisnes/Elitekollektivet/_apis/build/status/andreasisnes.Elitekollektivet.Pagerank?branchName=master)](https://dev.azure.com/andreasisnes/Elitekollektivet/_build/latest?definitionId=10&branchName=master)
[![NuGet Version](https://img.shields.io/nuget/v/Elitekollektivet.Pagerank?style=plastic)](https://www.nuget.org/packages/Elitekollektivet.Pagerank/)
[![NuGet Download](https://img.shields.io/nuget/dt/Elitekollektivet.Pagerank)](https://www.nuget.org/packages/Elitekollektivet.Pagerank/)
[![NuGet Download](https://img.shields.io/azure-devops/coverage/andreasisnes/Elitekollektivet/10/master)](https://dev.azure.com/andreasisnes/Elitekollektivet/_build/latest?definitionId=10&branchName=master)

# Introduction 
PageRank is a link analysis algorithm and it assigns a numerical weighting to each element of a hyperlinked set of documents, such as the World Wide Web, with the purpose of "measuring" its relative importance within the set. The algorithm may be applied to any collection of entities with reciprocal quotations and references. [[1]](#1)

![[[2]](#2)](https://www.globalsoftwaresupport.com/wp-content/uploads/2019/09/Untitled-min-600x271.jpg)

# Getting Started
## Installation
Via [NuGet](https://www.nuget.org/packages/Elitekollektivet.Pagerank/)

### Package Manager
```bash
Install-Package Elitekollektivet.Pagerank -Version {VERSION}
```

### .NET CLI
```bash
dotnet add package Elitekollektivet.Pagerank --version {VERSION}`
```

### PackageReference
```xml
<PackageReference Include="Elitekollektivet.Pagerank" Version="{VERSION}" />
```

### Paket CLI
```bash
paket add Elitekollektivet.Pagerank --version ${VERSION}
```
The newest version is displayed in the nuget badge.

## Usage
```C#
        static void Main(string[] args)
        {
            var data = new double[,]
            {
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 },
                { .2, .2, 2, 3 },
                { .5, .3, 2, 0 },
            };

            var result = new PagerankBuilder(new PagerankOptions
            {
                ConvergenceRate = .85,
                MakeIrreducible = true,
                MakeStochastic = true,
                Iterations = 100,
            }).Build().SetLinkMatrix(data).Run();
            
            for (int i = 0; i <= result.GetUpperBound(0); i++)
            {
                System.Console.WriteLine(result[i, 0]);
            }
        }
```

```bash
# stdout
0.37322354771784405
0.3666104771784249
0.15124481327800834
0.10892116182572618
```

## References
<a id="1">[1]</a> 
https://en.wikipedia.org/wiki/PageRank

<a id="2">[2]</a> 
https://www.globalsoftwaresupport.com/wp-content/uploads/2019/09/Untitled-min-600x271.jpg
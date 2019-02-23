# WixSharpPlayground
Experimenting with WixSharp open source project

I created a few projects
* WixSharpPlayground - the product/main app
* SomeTool - some tool deployed with the product
* SomeOtherTool - some other tool deployed with the product

I wanted the installer to
* Automatically discover all of the files for each part of the product from their respective project output folders, without any custom build steps or changing where binaries are copied to
* Install different parts of the product to different folders within the "main" installation folder
* Be as easy to build as compiling code in visual studio

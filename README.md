# Adding new test scenario

1. Create a folder in the root of the repository: {TestName}.
2. Put your functions code in {TestName}/Content

3. Add a scenario config: {TestName}/config.json:
```
{
    "runtime": "dotnet",            // FUNCTIONS_WORKER_RUNTIME app setting
    "description": "C# Ping",       // Short description
    "isScript": true                // Set it to false for a precompiled or java function. The content folder will be automatically builded to binaries.
}
```
4. Add a jmx definition file {TestName}/test.jmx:
   Note jmx definition uses custom variables from the file in agent:
```
<stringProp name="HTTPSampler.domain">${APP}.azurewebsites.net</stringProp>
...
<stringProp name="HTTPSampler.path">/api/HttpTrigger?count=1000&amp;code=${KEY}</stringProp>
```
{APP} - function app name, {KEY} - functions default key. 

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Repo for Playwright E2E tests

## Run on your machine
Open the solution in Visual Studio or 
```
dotnet restore
dotnet build
```

After that you should install the playwright stuff (browsers etc)

It installed a handy powershell in your project's bin directory that you should run.
```
pwsh .\bin\Debug\net6.0\playwright.ps1 install
```

Now you can run the tests from command line or VS.
```
dotnet test
```

Bonus: You can also run a code generator to scaffold a test flow:
```
.\bin\Debug\net6.0\playwright.ps1 codegen
```

## Azure DevOps pipeline
This pipeline is executed after deployment:

[azure-pipelines.yml](azure-pipelines.yml)

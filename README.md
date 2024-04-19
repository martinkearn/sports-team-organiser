# Sports Team Organiser
A website for organising a sports team including who is playing and their payments.

## Offline Development
These steps are for Visual Studio Code but you can substitue these IDEs as required.

- Install the Azurite Visual Studio Code extension
- In Visual Studio Code, start Azurite by typing `Azurite: Start` the Command Pallette
- Create a copy of `/src/STO.Api/appsettings.json` and name it `/src/STO.Api/appsettings.development.json` (this is a local file which is ignored by git)
- In `appsettings.development.json`, set the value of `StorageConfiguration:ConnectionString` to the generic Azureite connection string which is `DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;`
- Run the API by executing `dotnet run` in the terminal in the `/src/STO.Api` folder. You should see "Now listening on: http://localhost:5228"
- Run the STO.Wasm Blazor app via a terminal or different IDE sch as Rider. When running in localhost, it is automatically set to use the localhost API URL of http://localhost:5228

> By default, the Azureite tables will be empty so you will need to either create data or import it via something like Azure Storage Explorer.

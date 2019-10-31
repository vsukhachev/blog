FROM mcr.microsoft.com/dotnet/core/sdk:2.2

WORKDIR /dictionary

COPY . .

RUN dotnet restore

RUN dotnet publish -o /publish

ENTRYPOINT ["dotnet", "/publish/DictionaryApi.dll"]
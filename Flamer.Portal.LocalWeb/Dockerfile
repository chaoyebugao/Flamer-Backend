#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Flamer.Portal.LocalWeb/Flamer.Portal.LocalWeb.csproj", "Flamer.Portal.LocalWeb/"]
COPY ["Flamer.Model.Result/Flamer.Model.Result.csproj", "Flamer.Model.Result/"]
COPY ["Flamer.Model.Pagination/Flamer.Model.Pagination.csproj", "Flamer.Model.Pagination/"]
COPY ["Flamer.Service.Domain/Flamer.Service.Domain.csproj", "Flamer.Service.Domain/"]
COPY ["Flamer.Utility/Flamer.Utility.csproj", "Flamer.Utility/"]
COPY ["Flamer.Service.OSS/Flamer.Service.OSS.csproj", "Flamer.Service.OSS/"]
COPY ["Flamer.Data/Flamer.Data.csproj", "Flamer.Data/"]
COPY ["Flamer.Model.Web/Flamer.Model.Web.csproj", "Flamer.Model.Web/"]
COPY ["Flamer.Model.ViewModel/Flamer.Model.ViewModel.csproj", "Flamer.Model.ViewModel/"]
COPY ["Flamer.Service.Common/Flamer.Service.Common.csproj", "Flamer.Service.Common/"]
COPY ["Flamer.Service.Email/Flamer.Service.Email.csproj", "Flamer.Service.Email/"]
COPY ["Flamer.Service.ImageProxy/Flamer.Service.ImageProxy.csproj", "Flamer.Service.ImageProxy/"]
RUN dotnet restore "Flamer.Portal.LocalWeb/Flamer.Portal.LocalWeb.csproj"
COPY . .
WORKDIR "/src/Flamer.Portal.LocalWeb"
RUN dotnet build "Flamer.Portal.LocalWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Flamer.Portal.LocalWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Flamer.Portal.LocalWeb.dll"]
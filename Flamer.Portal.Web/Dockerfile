# 构建修改参考于：https://www.cnblogs.com/kklldog/p/netcore-docker-image-size.html
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["Flamer.Portal.Web/Flamer.Portal.Web.csproj", "Flamer.Portal.Web/"]
COPY ["Flamer.Service.Email/Flamer.Service.Email.csproj", "Flamer.Service.Email/"]
COPY ["Flamer.Service.Common/Flamer.Service.Common.csproj", "Flamer.Service.Common/"]
COPY ["Flamer.Service.Domain/Flamer.Service.Domain.csproj", "Flamer.Service.Domain/"]
COPY ["Flamer.Utility/Flamer.Utility.csproj", "Flamer.Utility/"]
COPY ["Flamer.Service.OSS/Flamer.Service.OSS.csproj", "Flamer.Service.OSS/"]
COPY ["Flamer.Data/Flamer.Data.csproj", "Flamer.Data/"]
COPY ["Flamer.Service.ImageProxy/Flamer.Service.ImageProxy.csproj", "Flamer.Service.ImageProxy/"]
RUN dotnet restore "Flamer.Portal.Web/Flamer.Portal.Web.csproj"
COPY . .
WORKDIR "/src/Flamer.Portal.Web"
RUN dotnet build "Flamer.Portal.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Flamer.Portal.Web.csproj" -c Release -o /app/publish \
	--runtime alpine-x64 \
    --self-contained true \
    /p:PublishTrimmed=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Flamer.Portal.Web.dll"]
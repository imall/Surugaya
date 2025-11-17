# 使用 .NET 8.0 SDK 作為建置階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 設定工作目錄
WORKDIR /src

# 複製 global.json 和解決方案檔案
COPY ["global.json", "."]
COPY ["Surugaya.sln", "."]

# 複製專案檔案
COPY ["Surugaya.API/Surugaya.API.csproj", "Surugaya.API/"]
COPY ["Surugaya.Common/Surugaya.Common.csproj", "Surugaya.Common/"]
COPY ["Surugaya.Repository/Surugaya.Repository.csproj", "Surugaya.Repository/"]
COPY ["Surugaya.Service/Surugaya.Service.csproj", "Surugaya.Service/"]

# 還原專案依賴
RUN dotnet restore "Surugaya.API/Surugaya.API.csproj"

# 複製所有原始碼
COPY . .

# 建置並發布專案
WORKDIR "/src/Surugaya.API"
RUN dotnet publish "Surugaya.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 使用 ASP.NET Core Runtime 作為執行時期階段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# 設定工作目錄
WORKDIR /app

# 暴露連接埠 (預設 ASP.NET Core 連接埠)
EXPOSE 8080
EXPOSE 8081

# 複製建置結果
COPY --from=build /app/publish .

# 設定進入點
ENTRYPOINT ["dotnet", "Surugaya.API.dll"]

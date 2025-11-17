# Surugaya API Docker 部署腳本

# 建置 Docker 映像
Write-Host "正在建置 Docker 映像..." -ForegroundColor Green
docker build -t surugaya-api:latest .

if ($LASTEXITCODE -eq 0) {
    Write-Host "Docker 映像建置成功!" -ForegroundColor Green
    
    # 顯示映像資訊
    Write-Host "映像資訊:" -ForegroundColor Yellow
    docker images surugaya-api:latest
    
    Write-Host ""
    Write-Host "部署選項:" -ForegroundColor Cyan
    Write-Host "1. 使用 docker run 啟動容器:" -ForegroundColor White
    Write-Host "   docker run -d -p 5000:8080 --name surugaya-api surugaya-api:latest" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. 使用 docker-compose 啟動:" -ForegroundColor White
    Write-Host "   docker-compose up -d" -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. 推送到 Docker Registry (可選):" -ForegroundColor White
    Write-Host "   docker tag surugaya-api:latest your-registry/surugaya-api:latest" -ForegroundColor Gray
    Write-Host "   docker push your-registry/surugaya-api:latest" -ForegroundColor Gray
} else {
    Write-Host "Docker 映像建置失敗!" -ForegroundColor Red
    exit 1
}

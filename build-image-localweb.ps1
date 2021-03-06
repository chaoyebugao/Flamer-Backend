

${CI_REGISTRY_IMAGE} = "flamer-localweb"

$versionTag = "1.1.7"

$DesktopPath = [Environment]::GetFolderPath("Desktop")

echo 镜像:${CI_REGISTRY_IMAGE}
echo 版本:$versionTag
docker rmi ${CI_REGISTRY_IMAGE}:$versionTag
docker build --rm -f Flamer.Portal.LocalWeb/Dockerfile -t ${CI_REGISTRY_IMAGE}:$versionTag .
docker save -o $DesktopPath/${CI_REGISTRY_IMAGE}-$versionTag.tar ${CI_REGISTRY_IMAGE}:$versionTag

Write-Output ''
Write-Output ''
Write-Output '------执行完毕------'
cmd /c pause | out-null

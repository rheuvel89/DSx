dotnet clean -c Release
dotnet build -c Release
7z a DSx.zip .\DSx.App\bin\Release\net6.0\*
scp DSx.zip rheuvel@20.105.188.65:E:\
ssh rheuvel@20.105.188.65 "7z e E:\DSx.zip" -oE:\DSx -y

robocopy ..\ServerManager\7zip    bin\Debug\net8.0\7zip      *.* /mir
robocopy ..\ServerManager\Icon    bin\Debug\net8.0\Icon      *.* /mir
robocopy ..\ServerManager\KeePass bin\Debug\net8.0\KeePass   *.* /mir

robocopy ..\ServerManager\7zip    bin\Release\net8.0\7zip    *.* /mir
robocopy ..\ServerManager\Icon    bin\Release\net8.0\Icon    *.* /mir
robocopy ..\ServerManager\KeePass bin\Release\net8.0\KeePass *.* /mir

exit /B 0
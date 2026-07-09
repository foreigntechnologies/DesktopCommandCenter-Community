$path = 'MainWindow.xaml'
$bytes = [System.IO.File]::ReadAllBytes($path)
if ($bytes[0] -ne 239 -or $bytes[1] -ne 187 -or $bytes[2] -ne 191) {
    $bom = [byte[]](239, 187, 191)
    [System.IO.File]::WriteAllBytes($path, $bom + $bytes)
}
$path = 'MainWindow.xaml.cs'
$bytes = [System.IO.File]::ReadAllBytes($path)
if ($bytes[0] -ne 239 -or $bytes[1] -ne 187 -or $bytes[2] -ne 191) {
    $bom = [byte[]](239, 187, 191)
    [System.IO.File]::WriteAllBytes($path, $bom + $bytes)
}

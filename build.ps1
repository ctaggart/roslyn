# the version in progress, used by pre-release builds
$version = '1.1.0'

$exit = 0
trap [Exception] {
    $exit++
    Write-Warning $_.Exception.Message
    continue
}

$pst = [TimeZoneInfo]::FindSystemTimeZoneById('Pacific Standard Time')
$date = New-Object DateTimeOffset([TimeZoneInfo]::ConvertTimeFromUtc([DateTime]::UtcNow, $pst), $pst.BaseUtcOffset)
$v = $version + '-a' + $date.ToString('yyMMddHHmm')

if ($env:appveyor){
	$v = $version + '-b' + [int]::Parse($env:appveyor_build_number).ToString('000')

    # install SourceLink.exe
    choco install SourceLink -y
}

echo '--> build'
msbuild .\src\Compilers\Core\Portable\CodeAnalysis.csproj /v:m /p:Configuration=Release

# TODO SourceLink needs to support project.json projects
#echo '--> source index'
#$u = 'https://raw.githubusercontent.com/ctaggart/roslyn/{0}/%var2%'
#$pp = 'Configuration Release'
#$sl = "SourceLink index -u '$u' -pp $pp -pr"
#iex "$sl .\src\Compilers\Core\Portable\CodeAnalysis.csproj"

echo '--> create nuget package'
if ($env:appveyor_repo_tag -eq 'true' -and $env:appveyor_repo_tag_name.StartsWith('v')){
    $v = $env:appveyor_repo_tag_name.Substring(1)
}
.\paket.bootstrapper.exe
.\paket.exe pack output bin version $v
echo "created $pwd\bin\Roslyn.CodeAnalysis.Internals.$v.nupkg"

exit $exit
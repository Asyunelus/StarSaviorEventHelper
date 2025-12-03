using System.Diagnostics;
using System.IO.Compression;

void Pause()
{
    Console.WriteLine("계속하려면 아무 키나 누르세요...");
    Console.ReadKey(true);
}

if (args.Length <= 0)
{
    Console.WriteLine("사용법 : Update.exe <압축파일명>");
    Pause();
    return;
}

string zipName = args[0];

List<string> excludedFiles = new List<string> {
    "Update.exe",
    "Update.dll",
    "Update.deps.json",
    "Update.runtimeconfig.json"
};

if (File.Exists(zipName))
{
    Console.WriteLine($"업데이트 파일 \"{zipName}\"을 발견했습니다.");

    try
    {
        string extractPath = Environment.CurrentDirectory;

        Console.WriteLine($"대상 폴더: {extractPath}");
        Console.WriteLine("압축 해제를 시작합니다...");
        using (ZipArchive archive = ZipFile.OpenRead(zipName))
        {
            int extractedCount = 0;

            foreach (var entry in archive.Entries)
            {
                string entryFileName = Path.GetFileName(entry.FullName);

                if (excludedFiles.Contains(entryFileName, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"  [SKIP] {entry.FullName}");
                    continue;
                }

                if (string.IsNullOrEmpty(entryFileName))
                {
                    string fullDirPath = Path.Combine(extractPath, entry.FullName);
                    if (!Directory.Exists(fullDirPath))
                    {
                        Directory.CreateDirectory(fullDirPath);
                    }
                    continue;
                }

                string destinationPath = Path.Combine(extractPath, entry.FullName);

                // 파일이 존재하는지 확인하고, 덮어쓰기
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                entry.ExtractToFile(destinationPath, overwrite: true);

                extractedCount++;
            }
        }
        Console.WriteLine("업데이트 성공: 압축 파일 내용이 성공적으로 해제되었습니다.");
        File.Delete(zipName);
        Thread.Sleep(1000);
        Process.Start("StarSaviorEvent.exe");
    }
    catch (Exception ex)
    {
        Console.WriteLine("업데이트 실패: 압축 해제 중 오류가 발생했습니다.");
        Console.WriteLine($"오류 내용: {ex.Message}");

        Pause();
    }
}
else
{
    Console.WriteLine($"업데이트 실패 : 파일 \"{zipName}\"이 존재하지 않습니다.");
    Pause();
}
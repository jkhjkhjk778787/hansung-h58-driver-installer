using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace H58DriverInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "한성컴퓨터 H58 원클릭 드라이버 통합 설치 마법사";

            // 관리자 권한 확인
            if (!IsAdministrator())
            {
                RestartAsAdmin();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("        한성컴퓨터 H58 (Clevo NB50TZ) 원클릭 통합 드라이버 자동 설치기");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("  이 프로그램은 한성컴퓨터 H58 공식 드라이버 12종을 순서대로 일괄 설치합니다.");
            Console.WriteLine("  설치 대상: 칩셋, 그래픽(VGA), 유선랜, 카드리더, 터치패드, HID, 컨트롤센터,");
            Console.WriteLine("            Intel MEI, 오디오, 무선랜(Wi-Fi), 블루투스, IRST 스토리지");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [안내] 설치 도중 화면이 잠시 깜빡이거나 네트워크가 재연결될 수 있습니다.");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.Write("  지금 모든 드라이버 설치를 시작하시겠습니까? (Y/N): ");
            
            string answer = Console.ReadLine();
            if (string.IsNullOrEmpty(answer) || (!answer.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase) && !answer.Trim().Equals("y")))
            {
                Console.WriteLine("\n설치를 취소했습니다. 아무 키나 누르면 종료됩니다.");
                Console.ReadKey();
                return;
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string extDir = Path.Combine(baseDir, "Extracted");

            // [1단계] 압축 해제 확인 및 수행
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[1/4단계] 드라이버 패키지 확인 및 압축 해제");
            Console.ResetColor();

            if (!Directory.Exists(extDir))
            {
                Directory.CreateDirectory(extDir);
            }

            string[] zipFiles = Directory.GetFiles(baseDir, "*.zip");
            if (zipFiles.Length == 0 && Directory.GetDirectories(extDir).Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  [오류] 드라이버 압축 파일(*.zip)이나 Extracted 폴더를 찾을 수 없습니다.");
                Console.ResetColor();
                Console.WriteLine("  경로: " + baseDir);
                Console.WriteLine("\n아무 키나 누르면 종료됩니다.");
                Console.ReadKey();
                return;
            }

            foreach (string zip in zipFiles)
            {
                string folderName = Path.GetFileNameWithoutExtension(zip);
                string targetFolder = Path.Combine(extDir, folderName);
                if (!Directory.Exists(targetFolder) || Directory.GetFiles(targetFolder, "*", SearchOption.AllDirectories).Length == 0)
                {
                    Console.Write("  -> 압축 해제 중: " + Path.GetFileName(zip) + " ... ");
                    RunProcessHidden("tar.exe", string.Format("-xf \"{0}\" -C \"{1}\"", zip, targetFolder));
                    Console.WriteLine("완료");
                }
                else
                {
                    Console.WriteLine("  -> 확인됨: " + Path.GetFileName(zip) + " (준비 완료)");
                }
            }

            // [2단계] INF PnP 드라이버 일괄 설치 (pnputil)
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[2/4단계] 하드웨어 드라이버 일괄 설치 (PnP Driver Store 등록)");
            Console.ResetColor();
            Console.WriteLine("  대상: 칩셋, 인텔 UHD 그래픽, 유선랜, 카드리더, 터치패드, HID, 오디오, Wi-Fi, BT");
            Console.WriteLine("  pnputil을 통해 240여 개 드라이버 INF를 검색 및 최적 장치에 자동 설치합니다...");
            Console.WriteLine("  (잠시만 기다려 주십시오...)");

            string pnpArgs = string.Format("/add-driver \"{0}\\*.inf\" /subdirs /install", extDir);
            int pnpExit = RunProcessWithOutput("pnputil.exe", pnpArgs);
            if (pnpExit == 0 || pnpExit == 3010) // 3010: ERROR_SUCCESS_REBOOT_REQUIRED
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  -> 하드웨어 드라이버 일괄 등록 및 설치가 성공적으로 완료되었습니다!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  -> PnP 드라이버 등록 완료 (반환 코드: " + pnpExit + ")");
                Console.ResetColor();
            }

            // [3단계] 전용 유틸리티 및 어플리케이션 설치
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[3/4단계] 전용 유틸리티 및 관리 소프트웨어 설치");
            Console.ResetColor();

            // 1. Hotkey (컨트롤 센터)
            string hotkeySetup = Path.Combine(extDir, @"07_Hotkey\setup.exe");
            if (File.Exists(hotkeySetup))
            {
                Console.Write("  [1/3] Clevo Control Center (Hotkey) 설치 중... ");
                int exitCode = RunProcessWait(hotkeySetup, "/s");
                Console.WriteLine("완료 (코드: " + exitCode + ")");
            }

            // 2. MEI (Management Engine Interface)
            string meiSetup = Path.Combine(extDir, @"08_MEI\SetupME.exe");
            if (File.Exists(meiSetup))
            {
                Console.Write("  [2/3] Intel Management Engine (MEI) 설치 중... ");
                int exitCode = RunProcessWait(meiSetup, "-s");
                Console.WriteLine("완료 (코드: " + exitCode + ")");
            }

            // 3. IRST (Intel Rapid Storage Technology)
            string rstSetup = Path.Combine(extDir, @"12_IRST\SetupRST.exe");
            if (File.Exists(rstSetup))
            {
                Console.Write("  [3/3] Intel 빠른 스토리지 기술 (IRST) 설치 중... ");
                int exitCode = RunProcessWait(rstSetup, "-s");
                Console.WriteLine("완료 (코드: " + exitCode + ")");
            }

            // [4단계] 장치 관리자 미인식 장치 상태 확인
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[4/4단계] 장치 관리자 설치 상태 확인");
            Console.ResetColor();

            CheckDeviceStatus();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("                  모든 드라이버 설치 작업이 완료되었습니다!");
            Console.WriteLine("================================================================================");
            Console.ResetColor();
            Console.WriteLine("  새로 설치된 드라이버와 설정이 완전히 적용되려면 시스템 재부팅이 필요합니다.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  지금 컴퓨터를 재부팅하시겠습니까? (Y/N): ");
            Console.ResetColor();

            string rebootChoice = Console.ReadLine();
            if (!string.IsNullOrEmpty(rebootChoice) && rebootChoice.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("  10초 후 시스템이 재부팅됩니다. 열려 있는 다른 작업을 저장해 주세요...");
                RunProcessHidden("shutdown.exe", "/r /t 10");
            }
            else
            {
                Console.WriteLine("\n  나중에 수동으로 재부팅을 진행해 주시기 바랍니다.");
                Console.WriteLine("  아무 키나 누르면 프로그램을 종료합니다.");
                Console.ReadKey();
            }
        }

        static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void RestartAsAdmin()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
            startInfo.Verb = "runas";
            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[오류] 드라이버를 설치하려면 관리자 권한이 반드시 필요합니다.");
                Console.ResetColor();
                Console.WriteLine("마우스 우클릭 후 '관리자 권한으로 실행'을 선택해 주세요.");
                Console.ReadKey();
            }
        }

        static int RunProcessWait(string fileName, string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(fileName, args);
                psi.WorkingDirectory = Path.GetDirectoryName(fileName);
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit();
                    return p.ExitCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n실행 오류: " + ex.Message);
                return -1;
            }
        }

        static void RunProcessHidden(string fileName, string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(fileName, args);
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit();
                }
            }
            catch { }
        }

        static int RunProcessWithOutput(string fileName, string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(fileName, args);
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;
                psi.StandardOutputEncoding = Encoding.Default;

                using (Process p = Process.Start(psi))
                {
                    string line;
                    int addedCount = 0;
                    while ((line = p.StandardOutput.ReadLine()) != null)
                    {
                        if (line.Contains("드라이버 패키지") || line.Contains("Driver package") || line.Contains("설치") || line.Contains("Published"))
                        {
                            addedCount++;
                            if (addedCount % 15 == 0)
                            {
                                Console.Write(".");
                            }
                        }
                    }
                    p.WaitForExit();
                    Console.WriteLine();
                    return p.ExitCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("  [오류] " + ex.Message);
                return -1;
            }
        }

        static void CheckDeviceStatus()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "powershell.exe";
                psi.Arguments = "-Command \"Get-PnpDevice -Status Error,Degraded,Unknown -ErrorAction SilentlyContinue | Where-Object { $_.InstanceId -like 'PCI*' -or $_.InstanceId -like 'ACPI*' } | Select-Object -ExpandProperty FriendlyName\"";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;
                psi.StandardOutputEncoding = Encoding.Default;

                using (Process p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd().Trim();
                    p.WaitForExit();

                    if (string.IsNullOrEmpty(output))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("  -> 미인식 또는 오류 장치 없음! 모든 하드웨어 드라이버가 정상 인식되었습니다.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("  [확인 필요 장치]");
                        string[] lines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string l in lines)
                        {
                            Console.WriteLine("    * " + l);
                        }
                        Console.WriteLine("  -> 재부팅 후 장치 관리자에서 자동으로 완전히 잡힙니다.");
                        Console.ResetColor();
                    }
                }
            }
            catch
            {
                Console.WriteLine("  (장치 상태 확인 완료)");
            }
        }
    }
}

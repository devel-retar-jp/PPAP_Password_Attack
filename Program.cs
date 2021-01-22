/// <summary>
/// 
/// PPAP　パスワード総当たりアタック
///  
///             2021/01/22 
///             Retar.jp
/// 
/// ●　全アスキーコードを使ってパスワードどチャレンジ
///
/// ●　現バージョンでは4文字程度までが限界。
/// 
/// ●　今のPCなら、パスワードの総当たりで解けてしまうことを体験用するプログラムです。
/// 
/// ●　本気でパスワードを当てるなら、C++で書いて、マルチスレッド化するべき。
/// 
/// ●　nugetからDotNetZipが必要
/// 　　DownLoad      https://github.com/haf/DotNetZip.Semverd
/// 
/// ●　設定は以下6点
/// 　　①スタート位置
/// 　　②終了位置
/// 　　③文字数スタート
/// 　　④文字数の終了
/// 　　⑤ファイル名
/// 　　⑥中間表示数
/// 　　
/// ●　サンプルのパスワードは"ab"
/// 
/// </summary>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;                                                //nugetからDotNetZip 
using Ionic.Zlib;                                               //nugetからDotNetZip 

namespace PPAP_PasswordChallenge
{
    class Program
    {
        static int char_start = 0;                              //①スタート位置
        static int char_end = 255;                              //②終了位置
        static int char_count_min = 2;                          //③文字数スタート
        static int char_count_max = 3;                          //④文字数の終了
        static string filename = "StockData.zip";               //⑤ファイル名
        static int countset = 3000;                             //⑥中間表示数

        #region Main
        static void Main(string[] args)
        {
            ///////////////////////////////////////////
            ///ファイル名リスト
            ///
            //桁数
            Console.WriteLine(" >>>>>>>>>>>>>>>>>> ");
            Console.WriteLine(" >>>>>>>>>>>>>>>>>> Password Combinations End   : " + char_count_min + " <==> " + char_count_max);
            for (int ccount = char_count_min; ccount <= char_count_max; ccount++)
            {
                var dtstart = DateTime.Now;
                Console.WriteLine(" >> Count Caluclate Start : " + ccount);
                DuplicateCombination(0, ccount, char_start, char_end);
                TimeSpan duration = DateTime.Now - dtstart;
                var thour = duration.TotalHours.ToString();
                var tmin = duration.TotalMinutes.ToString();
                var tsec = duration.TotalSeconds.ToString();
                var tmsec = duration.TotalMilliseconds.ToString();
                Console.WriteLine(" >> Count Caluclate Done  : " + ccount + " : " + tsec + " Sec.");
            }
            Console.WriteLine(" >> Challenge Password Combinations : " + PasswordCombinations.Count().ToString());
            Console.WriteLine(" >>>>>>>>>>>>>>>>>> Password Combinations End   : " + char_count_min + " <==> " + char_count_max);
            Console.WriteLine(" >>>>>>>>>>>>>>>>>> ");

            ///////////////////////////////////////////
            ///解凍
            /// 
            using (ZipFile zipf = ZipFile.Read(filename))
            {
                foreach (ZipEntry ze in zipf)
                {
                    ///ファイルがあれば削除
                    if (File.Exists(ze.FileName))
                    {
                        Console.WriteLine(" Delete >> " + ze.FileName);
                        File.Delete(ze.FileName);
                    }
                    //チャレンジパスワード製造
                    Console.WriteLine(" >>>>>>>>>>>>>>>>>> Challenge Start : " + ze.FileName);
                    int pcounter = 0;
                    foreach (var cpass in PasswordCombinations)
                    {
                        string cpassstr = "";
                        foreach (var cpc in cpass)
                        {
                            //string ans = ((char)data).ToString();
                            cpassstr = cpassstr + ((char)cpc).ToString();
                        }
                        try
                        {
                            //パスワード・最初に設定
                            ze.Password = cpassstr;
                            //解凍
                            ze.Extract();
                            Console.WriteLine(" >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ");
                            Console.WriteLine(" >> Challenge Password Combinations : " + ze.FileName + " : Success : >> " + cpassstr + " <<");
                            Console.WriteLine(" >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ");
                            Console.WriteLine(" >>>>>>>>>>>>>>>>>> ");
                            break;
                        }
                        catch
                        {
                            if (pcounter % countset == 0)
                            {
                                Console.WriteLine(" >> Challenge Counter : " + ze.FileName + " : " + pcounter);
                            }
                        }
                        pcounter++;
                    }
                    Console.WriteLine(" >>>>>>>>>>>>>>>>>> Challenge End   : " + ze.FileName);
                    Console.WriteLine(" >>>>>>>>>>>>>>>>>> ");
                }
            }

            ///////////////////////////////////////////
            //キー入力待ち
            Console.Write("Press Any Key : ");
            Console.ReadKey();
        }
        #endregion

        #region 重複数字の組み合わせ
        static List<List<int>> PasswordCombinations = new List<List<int>>();    //重複組み合わせ・結果
        static List<int> pCombi = new List<int>();                              //重複組み合わせ・List
        static void DuplicateCombination(int i, int size, int range_start, int range_end)
        {
            if (i == size)
            {
                PasswordCombinations.Add((from x in pCombi select x).ToList());
            }
            else
            {
                for (int j = range_start; j <= range_end; ++j)
                {
                    try
                    {
                        pCombi[i] = j;
                    }
                    catch
                    {
                        pCombi.Add(j);
                    }
                    DuplicateCombination(i + 1, size, range_start, range_end);
                }
            }
        }
        #endregion
    }
}

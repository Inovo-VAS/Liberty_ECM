using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class NumberOps
    {
        private const string NewChar = "";
        private int maxLength15 { get; set; }
        private int maxLength13 { get; set; }

        public string FormatNumber(string inPhone)
        {
            try
            {
                SetMax(inPhone);
                inPhone = Regex.Replace(inPhone, "[^0-9]", "");
                inPhone = inPhone.Replace(" ", NewChar);
                inPhone = (inPhone.Length < 9) ? String.Empty : inPhone;
                if (inPhone != String.Empty)
                {
                    switch (inPhone)
                    {
                        case "000000000":
                            inPhone = String.Empty;
                            break;
                        case "0000000000":
                            inPhone = String.Empty;
                            break;
                        case "00000000000":
                            inPhone = String.Empty;
                            break;
                        default:
                            break;
                    }
                    if (inPhone != String.Empty)
                    {
                        string Partial = inPhone.Substring(0, 4);
                        string Partial1 = inPhone.Substring(0, 3);
                        SetMax(inPhone);
                        switch (Partial)
                        {
                            case "0026":
                                inPhone = (inPhone.Substring(0, 1) == "0") ? inPhone[1..maxLength15] : inPhone;
                                SetMax(inPhone);
                                inPhone = (inPhone.Length > 13) ? inPhone[1..maxLength13] : inPhone;
                                SetMax(inPhone);
                                break;
                            case "0027":
                                inPhone = (inPhone.Substring(0, 1) == "0") ? inPhone[1..maxLength15] : inPhone;
                                SetMax(inPhone);
                                inPhone = (inPhone.Length > 13) ? inPhone[1..maxLength13] : inPhone;
                                SetMax(inPhone);
                                break;
                            default:
                                if (Partial1 == "027")
                                {
                                    inPhone = inPhone[2..maxLength15];
                                    SetMax(inPhone);
                                    inPhone = (inPhone.Length > 13) ? inPhone[1..maxLength13] : inPhone;
                                    SetMax(inPhone);
                                    inPhone = (inPhone.Length < 9) ? String.Empty : inPhone;
                                    SetMax(inPhone);
                                }
                                else
                                {
                                    inPhone = (inPhone.Substring(0, 1) == "0") ? inPhone[1..maxLength15] : inPhone;
                                    SetMax(inPhone);
                                    inPhone = (inPhone.Substring(0, 2) == "00") ? inPhone[2..maxLength15] : inPhone;
                                    SetMax(inPhone);
                                    inPhone = (inPhone.Substring(0, 2) == "27") ? inPhone[2..maxLength15] : inPhone;
                                    SetMax(inPhone);
                                    inPhone = (inPhone.Length > 13) ? inPhone[1..maxLength13] : inPhone;
                                    SetMax(inPhone);
                                    inPhone = (inPhone.Length < 9) ? String.Empty : inPhone;
                                    SetMax(inPhone);
                                }
                                break;
                        }
                    }
                    if(inPhone != String.Empty)
                    {
                        inPhone = (inPhone.Substring(0, 3) == "264" || inPhone.Substring(0, 3) == "268" || inPhone.Substring(0, 3) == "267") ? "0" + inPhone : inPhone;
                        //inPhone = (inPhone.Substring(0, 3) == "648") ? "02" + inPhone : inPhone;
                    }
                    inPhone = (inPhone.Length < 9) ? String.Empty : inPhone;
                }
                return inPhone;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetMax(string inPhone)
        {
            this.maxLength15 = (inPhone.Length > 15) ? 15 : inPhone.Length;
            this.maxLength13 = (inPhone.Length > 13) ? 13 : inPhone.Length;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TW_POC
{
    public partial class CurrencyConversor : System.Web.UI.Page
    {
        protected System.Web.UI.HtmlControls.HtmlInputFile File1;
        protected System.Web.UI.HtmlControls.HtmlInputButton Submit1;
        private static Dictionary<string, int> roman = new Dictionary<string, int>();
        string SaveLocation = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                uploadFile();
            }
        }

        private void uploadFile()
        {
            if ((File1.PostedFile != null) && (File1.PostedFile.ContentLength > 0))
            {
                string fn = System.IO.Path.GetFileName(File1.PostedFile.FileName);
                SaveLocation = Server.MapPath("Uploaded") + "\\" + fn;
                try
                {
                    File1.PostedFile.SaveAs(SaveLocation);
                    Response.Write("The currency file has been uploaded with the following values:<br>");
                    readCurrencyFile();
                }
                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }
            }
            else
            {
                Response.Write("Please select a file to upload.");
            }
        }


        private void readCurrencyFile()
        {
            string line = "";
            
            Dictionary<string, int> conversor = new Dictionary<string, int>();
            Dictionary<string, bool> primitiveValues = new Dictionary<string, bool>();

            if (roman.Count == 0)
            {
                roman.Add("I", 1);
                roman.Add("V", 5);
                roman.Add("X", 10);
                roman.Add("L", 50);
                roman.Add("C", 100);
                roman.Add("D", 500);
                roman.Add("M", 1000);
            }

            using (StreamReader sr = new StreamReader(SaveLocation))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //Response.Write("Se trabajara con la siguiente linea: " + line + "<br>");
                    string[] lineArray =  line.Split(new string[] { " is " }, StringSplitOptions.None);

                    //If there is value assignment
                    if (lineArray != null && lineArray.Length > 0 && lineArray.Length == 2)
                    {
                        try
                        {
                            if (lineArray[0].Contains("Credits") || lineArray[1].Contains("?"))
                            {
                                //Calculate value
                                int counter = 0;
                                int newValue = 0;
                                string newEq = lineArray[1].Split('?')[0];
                                string[] eqArray = newEq.Split(' ');
                                foreach (string eqVariable in eqArray)
                                {
                                    if (conversor.ContainsKey(eqVariable))// conversor[eqVariable] > 0)
                                    {
                                        //aplicar criterio de suma de numeros romanos
                                        if (counter == 0)
                                            newValue = conversor[eqVariable];
                                        else
                                        {
                                            if (counter != eqArray.Length - 2)
                                            {
                                                //verificar que no se pongan vairables mas de 3 veces de corrido
                                                if (conversor[eqVariable] > conversor[eqArray[counter - 1]])
                                                {
                                                    newValue = conversor[eqVariable] - newValue;

                                                }
                                                else
                                                {
                                                    newValue += conversor[eqVariable];
                                                }
                                            }
                                            else
                                            {
                                                if(!primitiveValues[eqVariable])
                                                {
                                                    //se asume que las lineas vienen ordenadas, poniendo primero las variables que se llevan a numero romano y finalmente, una variable la cual es multiplicada por dicho numero
                                                    newValue = newValue * conversor[eqVariable];
                                                }
                                                else
                                                {
                                                    newValue += conversor[eqVariable];
                                                }
                                            }
                                        }
                                    }
                                    
                                    counter++;
                                }
                                Response.Write(newEq + " is " + newValue.ToString() + "<br>");
                            }
                            else
                            {
                                if (!lineArray[1].Contains("Credits"))
                                {
                                    //assignment line
                                    //glob is I
                                    conversor.Add(lineArray[0], roman[lineArray[1]]);
                                    primitiveValues.Add(lineArray[0], true);
                                }
                                else
                                {
                                    //equation line
                                    //pish glob glob Silver is 38 Credits
                                    string equation = lineArray[0];
                                    string strValue = lineArray[1].Replace("Credits", "").Trim();
                                    int value = Convert.ToInt32(strValue);

                                    //separar la equacion, ver terminos existentes, no existentes, aplicar criterio para conformar numero resultante
                                    //Los numeros romanos multiplican los terminos nuevos glob glob Silver = II Silver = 2 * Silver
                                    string[] eqArray = lineArray[0].Split(' ');
                                    int newValue = 0;
                                    int counter = 0;
                                    foreach (string eqVariable in eqArray)
                                    {
                                        //si se esta leyendo numero ya conocido
                                        if (conversor.ContainsKey(eqVariable))// conversor[eqVariable] > 0)
                                        {
                                            //aplicar criterio de suma de numeros romanos
                                            if(counter == 0)
                                                newValue = conversor[eqVariable];
                                            else
                                            {
                                                //verificar que no se pongan vairables mas de 3 veces de corrido
                                                if (conversor[eqVariable] > conversor[eqArray[counter - 1]])
                                                {
                                                    newValue = conversor[eqVariable] - newValue;
                                                }
                                                else
                                                {
                                                    newValue += conversor[eqVariable];
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //se lee nueva expresion
                                            //calcular valor expresion
                                            //agregar a diccionario conversor
                                            string newVariableStr = eqVariable;
                                            int newVariableValue = 0;
                                            int resultValueInLine = 0;

                                            //si se cae, arrojar error por ecuacion mal formada
                                            string resultString = Regex.Match(lineArray[1], @"\d+").Value;
                                            resultValueInLine = Int32.Parse(resultString);

                                            newVariableValue = (resultValueInLine / newValue);
                                            conversor.Add(newVariableStr, newVariableValue);
                                            primitiveValues.Add(newVariableStr, false);
                                            //Response.Write(newVariableStr + ": " + newVariableValue + "<br>");
                                        }
                                        counter++;
                                    }
                                    
                                }
                            }

                        }
                        catch
                        {
                            Response.Write("I have no idea what you are talking about. (" + line + ")");
                        }
                        
                    }
                    else
                    {
                        Response.Write("I have no idea what you are talking about. (" + line + ")");
                    }
                }
            }
        }






    }
}
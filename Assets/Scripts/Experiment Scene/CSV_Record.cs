using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CSVs
{
    // Check if the CSV file exists
    public static void Exist(string path, string tableHeader)
    {
        if (!File.Exists(path))
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            fs.Close();
            StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
            sw.Write(tableHeader);
            sw.Flush();
            sw.Close();
            Debug.Log($"CSV file not found, new one created:" + path);
            return;
        }
        else
        {
            Debug.Log("CSV file found");
        }
    }

    // Read the entire CSV
    public static string[][] Read(string path)
    {
        string[] lineData = File.ReadAllLines(path);
        var rd = new string[lineData.Length][];
        for (int i = 0; i < lineData.Length; i++)
        {
            rd[i] = lineData[i].Split(',');
        }
        return rd;
    }

    // Write the entire CSV
    public static void Write(string[][] sth, string path)
    {
        StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
        string data = "";
        int j = 0;
        for (int i = 0; i < sth.Length; i++)
        {
            data = "";
            for (j = 0; j < sth[i].Length; j++)
            {
                data += sth[i][j];
                if (j < sth[i].Length - 1) data += ",";
            }
            sw.WriteLine(data);
        }
        sw.Flush();
        sw.Close();
    }

    // Append a line to the CSV
    public static void AddLine(string[] str_add, string path)
    {
        var str = Read(path);
        var str_new = new string[str.Length + 1][];
        for (int i = 0; i < str_new.Length; i++)
        {
            if (i < str.Length)
            {
                str_new[i] = str[i];
            }
            if (i == str.Length)
            {
                str_new[i] = str_add;
            }
        }
        Write(str_new, path);
    }

    public static void AddLineWithStringList(List<string> str, string path)
    {
        string[] str_new = new string[str.Count];
        for (int i = 0; i < str_new.Length; i++)
        {
            str_new[i] = str[i];
        }
        AddLine(str_new, path);
    }
}

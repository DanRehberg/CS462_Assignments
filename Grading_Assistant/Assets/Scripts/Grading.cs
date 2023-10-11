using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Grading : MonoBehaviour
{
    public Material dir;
    public Material point;
    public Material spot;

    public TMPro.TextMeshProUGUI text;

    public bool makeTestFiles;

    public int underscoreParseCount;

    string gradingPath = @"Assets\Grading";
    string compressPath = @"Assets\Grading\compressed";
    string testsPath = @"Assets\Grading\tests";

    long dataLimit = 10000;

    const int totalDir = 20;
    const int totalPoint = 30;
    const int totalSpot = 40;
    const int noErr = 10;

    int curIndex = -1;
    int state = 0;
    bool csvRecorded = false;
    string[] testDirs;
    string[] student;
    int[] dirScore;
    int[] dirDeduction;
    int[] pointScore;
    int[] pointDeduction;
    int[] spotScore;
    int[] spotDeduction;
    int[] noErrScore;


    // Start is called before the first frame update
    void Start()
    {
        if (makeTestFiles)
        {
            text.text = "Decompressing files from Grading/compressed directory; check console output for issues and status";
            string fullpath = Path.Combine(Directory.GetCurrentDirectory(), compressPath);
            string dest = Path.Combine(Directory.GetCurrentDirectory(), testsPath);
            string[] files = Directory.GetFiles(fullpath);
            //Clear out files and directories in the testsPath for
            //  subsequent runs of makeTestFiles (could use enumerated..)
            try
            {
                string[] subdirectories = Directory.GetDirectories(dest);
                foreach (string s in subdirectories)
                {
                    Directory.Delete(s, true);
                }
            }
            catch (System.Exception err)
            {
                print(err.Message);
                print("ERROR OCCURRED WHEN PURGING OLD TEST DIRECTORIES, IS THE " + testsPath + " FOLDER MISSING??!!");
                print("Attempting to add the directory..");
                Directory.CreateDirectory(dest);
            }

            //Build all test files in directories by students' name
            foreach (string f in files)
            {
                if (Path.GetExtension(f).Contains("meta")) continue;
                long size = new FileInfo(f).Length;
                string name = Path.GetFileName(f);
                string[] splits = name.Split('_');
                name = "";
                for (int i = 0; i < underscoreParseCount; ++i)
                {
                    name += splits[i];
                    if ((i + 1) < underscoreParseCount) name += "_";
                }
                if (size > dataLimit)
                {
                    print("MANUALLY PULL AND MODIFY STUDENT: " + name + " FILES AS SIZE IS: " + size + " BYTES!!!!");
                    continue;
                }
                string testDir = Path.Combine(dest, name);
                Directory.CreateDirectory(testDir);
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(f, testDir);
                }
                catch (System.Exception err)
                {
                    print("UNZIP ERR for: " + name + " DELETING DIRECTORY, MANUAL TROUBLESHOOT REQUIRED!!!!");
                    Directory.Delete(testDir);
                }
            }
            //Modify all shader names to contain student's name
            string[] dirs = Directory.GetDirectories(dest);
            foreach (string d in dirs)
            {
                //Remove MACOS directory from previous archive
                string name = Path.GetFileName(d.TrimEnd('\\'));
                string[] badDir = Directory.GetDirectories(d, "__MACOSX", SearchOption.AllDirectories);
                foreach (string bad in badDir)
                {
                    Directory.Delete(bad, true);
                }
                //Remove any scripts as they could be duplicates or contain errors
                string[] badFiles = Directory.GetFiles(d, "*.cs", SearchOption.AllDirectories);
                foreach (string bad in badFiles)
                {
                    print("CHECK SUBMISSION FROM: " + name + " TO SEE IF THEY SUBMITTED WRONG FILES BECAUSE " + Path.GetFileName(bad) + " WAS FOUND!!!!");
                    File.Delete(bad);
                }
                //Rename shaders so they can be looked up easily
                //  Rename shaderLab files mostly for sanity
                string[] shaders = Directory.GetFiles(d, "*.shader", SearchOption.AllDirectories);
                foreach (string s in shaders)
                {
                    string data = File.ReadAllText(s);
                    data = data.Replace("Unlit/", "Unlit/" + name);
                    File.WriteAllText(s, data);
                    string filename = Path.GetFileName(s);
                    string newName = s.Replace(filename, name + filename);
                    File.Move(s, newName);
                }
            }
            print("PLEASE TURN OFF MAKE TEST FILE BOOLEAN :) AND RERUN Playmode in Unity Editor");

        }
        else
        {
            //Files should have already been created if in here
            string dest = Path.Combine(Directory.GetCurrentDirectory(), testsPath);
            testDirs = Directory.GetDirectories(dest);
            if (testDirs.Length == 0)
            {
                print("NO STUDENT DIRECTORIES FOUND, IS THIS THE CORRECT PATH AND WERE STUDENT FILES PREPROCCESSED?");
                makeTestFiles = true;//Prevent access to controls in Update method.
                return;
            }
            student = new string[testDirs.Length];
            dirScore = new int[testDirs.Length];
            dirDeduction = new int[testDirs.Length];
            pointScore = new int[testDirs.Length];
            pointDeduction = new int[testDirs.Length];
            spotScore = new int[testDirs.Length];
            spotDeduction = new int[testDirs.Length];
            noErrScore = new int[testDirs.Length];
            setBasePoints(testDirs);
        }
    }

    private void setBasePoints(string[] directories)
    {
        int index = 0;
        foreach (string d in directories)
        {
            string name = Path.GetFileName(d.TrimEnd('\\'));
            student[index] = name;
            dirScore[index] = totalDir;
            pointScore[index] = totalPoint;
            spotScore[index] = totalSpot;
            noErrScore[index] = noErr;
            string shaderPath = "";
            try
            {
                shaderPath = Directory.GetFiles(d, "*Dir.shader", SearchOption.AllDirectories)[0];
                dirDeduction[index] = (illegalHelpers(Path.Combine(d, shaderPath)) == false) ? 0 : -5;

                shaderPath = Directory.GetFiles(d, "*Point.shader", SearchOption.AllDirectories)[0];
                pointDeduction[index] = (illegalHelpers(Path.Combine(d, shaderPath)) == false) ? 0 : -5;

                shaderPath = Directory.GetFiles(d, "*Spot.shader", SearchOption.AllDirectories)[0];
                spotDeduction[index] = (illegalHelpers(Path.Combine(d, shaderPath)) == false) ? 0 : -5;
            }
            catch (System.Exception err)
            {
                print(err.Message + " student " + name + " might have missing shaders..");
            }
            index += 1;
        }
    }
    bool illegalHelpers(string shaderPath)
    {
        bool temp = false;
        string data = File.ReadAllText(shaderPath);

        // Current set of illegal helpers
        //      Append more as needed..//
        if (data.Contains("UNITY_MATRIX_MVP", System.StringComparison.InvariantCultureIgnoreCase)) temp = true;
        if (data.Contains("UNITY_MATRIX_MV", System.StringComparison.InvariantCultureIgnoreCase)) temp = true;
        if (data.Contains("UNITY_MATRIX_VP", System.StringComparison.InvariantCultureIgnoreCase)) temp = true;
        if (data.Contains("UnityObjectToClipPos", System.StringComparison.InvariantCultureIgnoreCase)) temp = true;
        if (data.Contains("UnityObjectToViewPos", System.StringComparison.InvariantCultureIgnoreCase)) temp = true;


        return temp;
    }

    void writeCSV()
    {
        if (csvRecorded) return;

        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), gradingPath);
        string data = "student, dir score, dir deduction, point score, point deduction, spot score, spot deduction, no error score";

        for (int i = 0; i < testDirs.Length; ++i)
        {
            data += "\n" + student[i] + ", " + (dirScore[i] + dirDeduction[i]).ToString() + ", ";
            data += ((dirDeduction[i] == 0) ? "false, " : "true, ") + (pointScore[i] + pointDeduction[i]).ToString() + ", ";
            data += ((pointDeduction[i] == 0) ? "false, " : "true, ") + (spotScore[i] + spotDeduction[i]).ToString() + ", ";
            data += ((spotDeduction[i] == 0) ? "false, " : "true, ") + noErrScore[i].ToString();
        }

        outputPath = Path.Combine(outputPath, "grades.csv");
        File.WriteAllText(outputPath, data);

        csvRecorded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!makeTestFiles)
        {
            if (curIndex < 0)
            {
                if (testDirs.Length == 0)
                {
                    text.text = "No Test Directories Found, did you run editor with makeTestFiles on?";
                }
                else
                {
                    text.text = "Press Space to start grading assistant. Note, if input isn't working, then click in the Game window's viewspace.";
                    if (Input.GetKeyDown(KeyCode.Space)) curIndex = 0;
                }
            }
            else
            {
                //State machine
                // 0 change shader and move to state 1
                // 1 Ask if there are any errors, once reported move to 2
                // 2 Ask if Dir (leftmost) looks correct (if not, flag with negative score), once reported move to 3
                // 3 Ask if Point (middle) looks correct (flag with negative if not), once reported move to 4
                // 4 Ask if Spot (rightmost) looks correct (flag with negative if not), once reported move to 5
                // 5 Ask if ready for next student or if need to restart, if restart move to 1, else increment index and move to 0
                switch (state)
                {
                    case 0:
                        if (curIndex >= testDirs.Length)
                        {
                            text.text = "All Students Recorded, CSV in Grading directory";

                            //Write results to CSV once
                            writeCSV();

                            print("All students recorded");
                            return;
                        }
                        //Change the shaders used
                        string name = Path.GetFileName(testDirs[curIndex].TrimEnd('\\'));
                        student[curIndex] = name;
                        //Check for violations in shaderLab files
                        Shader dirChange = Shader.Find("Unlit/" + name + "Dir");
                        Shader pointChange = Shader.Find("Unlit/" + name + "Point");
                        Shader spotChange = Shader.Find("Unlit/" + name + "Spot");
                        print("Changing to " + name + "\'s shaders");
                        if (dirChange == null)
                        {
                            Debug.LogError("\tCould not find " + name + "Dir");
                            dir.shader = Shader.Find("Unlit/Dir");
                        }
                        else dir.shader = dirChange;
                        if (pointChange == null)
                        {
                            Debug.LogError("\tCould not find " + name + "Point");
                            point.shader = Shader.Find("Unlit/Point");
                        }
                        else point.shader = pointChange;
                        if (spotChange == null)
                        {
                            Debug.LogError("\tCould not find " + name + "Spot");
                            spot.shader = Shader.Find("Unlit/Spot");
                        }
                        else spot.shader = spotChange;

                        state = 1;
                        break;

                    case 1:
                        text.text = "Were no errors reported in the console? 'y' for none, 'n' for errors found (would be under current student name)";
                        if (Input.GetKeyDown(KeyCode.Y))
                        {
                            noErrScore[curIndex] = noErr;
                            state = 2;
                        }
                        else if (Input.GetKeyDown(KeyCode.N))
                        {
                            noErrScore[curIndex] = 0;
                            state = 2;
                        }
                        break;

                    case 2:
                        text.text = "Does Dir (left most objects) look correct? 'y' for yes, 'n' for no (will need manual review later)";
                        if (Input.GetKeyDown(KeyCode.Y))
                        {
                            dirScore[curIndex] = totalDir;
                            state = 3;
                        }
                        else if (Input.GetKeyDown(KeyCode.N))
                        {
                            //Negative score is a flag indicating manual review
                            dirScore[curIndex] = -totalDir;
                            state = 3;
                        }
                        break;

                    case 3:
                        text.text = "Does Point (middle objects) look correct? 'y' for yes, 'n' for no (will need manual review later)";
                        if (Input.GetKeyDown(KeyCode.Y))
                        {
                            pointScore[curIndex] = totalPoint;
                            state = 4;
                        }
                        else if (Input.GetKeyDown(KeyCode.N))
                        {
                            //Negative score is a flag indicating manual review
                            pointScore[curIndex] = -totalPoint;
                            state = 4;
                        }
                        break;

                    case 4:
                        text.text = "Does Spot (right most objects) look correct? 'y' for yes, 'n' for no (will need manual review later)";
                        if (Input.GetKeyDown(KeyCode.Y))
                        {
                            spotScore[curIndex] = totalSpot;
                            state = 5;
                        }
                        else if (Input.GetKeyDown(KeyCode.N))
                        {
                            //Negative score is a flag indicating manual review
                            spotScore[curIndex] = -totalSpot;
                            state = 5;
                        }
                        break;

                    case 5:
                        text.text = "Move onto next student, or regrade this student? 'space' to move on, 'backspace' to regrade";
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            curIndex += 1;
                            state = 0;
                        }
                        else if (Input.GetKeyDown(KeyCode.Backspace))
                        {
                            state = 1;
                        }
                        break;
                }
            }
        }
    }
}

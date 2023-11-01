using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class P1_Grading : MonoBehaviour
{
    public bool makeTestFiles;

    public int underscoreParseCount;

    string gradingPath = @"Assets\Grading";
    string compressPath = @"Assets\Grading\compressed";
    string testsPath = @"Assets\Grading\tests";
    string colPath = @"Assets\Scripts\ColTester.cs";

    long dataLimit = 10000;

    string[] foldersToRemove;
    int folderRemoveCount = 0;

    const int AABB = 60;
    const int SPHERE = 30;
    const int noErr = 10;

    bool csvRecorded = false;
    string[] testDirs;
    string[] student;


    // Start is called before the first frame update
    void Start()
    {
        if (makeTestFiles)
        {
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
            //Modify all class names to contain student's name
            //  and create a test entry for the student in ColTester
            string testScript = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), colPath));
            int storedIndex = 0;
            string[] dirs = Directory.GetDirectories(dest);
            foldersToRemove = new string[dirs.Length];
            foreach (string d in dirs)
            {
                //Remove MACOS directory from previous archive
                string name = Path.GetFileName(d.TrimEnd('\\'));
                string[] badDir = Directory.GetDirectories(d, "__MACOSX", SearchOption.AllDirectories);
                foreach (string bad in badDir)
                {
                    Directory.Delete(bad, true);
                }
                //Modification from below:
                //  Do not delete the script we 
                //Remove any scripts as they could be duplicates or contain errors
                string[] badFiles = Directory.GetFiles(d, "*.cs", SearchOption.AllDirectories);
                foreach (string bad in badFiles)
                {
                    if (bad.Contains("CollisionTests")) continue;

                    print("CHECK SUBMISSION FROM: " + name + " TO SEE IF THEY SUBMITTED WRONG FILES BECAUSE " + Path.GetFileName(bad) + " WAS FOUND!!!!");
                    File.Delete(bad);
                }
                //Rename functions for static use, and class names for
                //  removing name collisions
                try
                {
                    string collision = Directory.GetFiles(d, "*.cs", SearchOption.AllDirectories)[0];
                    {
                        string data = File.ReadAllText(collision);
                        //If student provided a script that does not contain the files, discard this test entry from automation.
                        if (!data.Contains("collisionAABB") || !data.Contains("collisionVertexSphere")) throw new System.Exception("Necessary test function not found");
                        data = data.Replace("class CollisionTests", "class " + name + "CollisionTests");
                        data = data.Replace("public bool collisionVertexSphere", "public static bool collisionVertexSphere");
                        data = data.Replace("public bool collisionAABB", "public static bool collisionAABB");
                        File.WriteAllText(collision, data);
                        string filename = Path.GetFileName(collision);
                        string newName = collision.Replace(filename, name + filename);
                        File.Move(collision, newName);
                        testScript = testScript.Replace("//New Line", "try\n\t\t{\n\t\t\t//New Line");
                        string codeLine = "result0[" + storedIndex.ToString() + "] = " + name + "CollisionTests.collisionAABB(minA, maxA, minB, maxB);\n";
                        testScript = testScript.Replace("//New Line", codeLine + "\t\t\t//New Line");
                        codeLine = "result1[" + storedIndex.ToString() + "] = " + name + "CollisionTests.collisionAABB(minC, maxC, minB, maxB);\n";
                        testScript = testScript.Replace("//New Line", codeLine + "\t\t\t//New Line");
                        codeLine = "result2[" + storedIndex.ToString() + "] = " + name + "CollisionTests.collisionAABB(minC, maxC, minA, maxA);\n";
                        testScript = testScript.Replace("//New Line", codeLine + "\t\t\t//New Line");
                        codeLine = "result3[" + storedIndex.ToString() + "] = " + name + "CollisionTests.collisionVertexSphere(sphere, radA, testPoint);\n";
                        testScript = testScript.Replace("//New Line", codeLine + "\t\t\t//New Line");
                        codeLine = "result4[" + storedIndex.ToString() + "] = " + name + "CollisionTests.collisionVertexSphere(sphere, radB, testPoint);\n";
                        testScript = testScript.Replace("//New Line", codeLine + "\t\t//New Line");
                        testScript = testScript.Replace("//New Line", "}\n\t\tcatch(System.Exception err)\n\t\t{\t\t\terrThrown[" + 
                            storedIndex.ToString() + "] = true;\n\t\t}\n\t\t//New Line");
                        storedIndex += 1;
                    }
                }
                catch (System.Exception err)
                {
                    print(err.Message + " manual review required for " + name + " DELETING THEIR FOLDER FROM TEST REGION!!!");
                    foldersToRemove[folderRemoveCount++] = name;
                    Directory.Delete(d, true);
                }
            }
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), colPath), testScript);
            print("Expected count of tests: " + storedIndex + " from total: " + foldersToRemove.Length + " after removing: " + folderRemoveCount);
            print("Folders removed:");
            for (int i = 0; i < folderRemoveCount; ++i)
                print(foldersToRemove[i]);
            print("If more errs emerge that prevent execution when Unity reloads assets, remove folders and perform manual review for those submissions.");
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
            int itr = 0;
            foreach (string d in testDirs)
            {
                string name = Path.GetFileName(d.TrimEnd('\\'));
                student[itr++] = name;
            }
            ColTester.runTests(testDirs.Length);
            writeCSV();
        }
    }

    void writeCSV()
    {
        if (csvRecorded) return;

        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), gradingPath);
        string data = "student, aabb score, aabb deduction, sphere score, sphere deduction, no error score";

        for (int i = 0; i < testDirs.Length; ++i)
        {
            int aabbScore = ((ColTester.result0[i] == ColTester.solution0 &&
                ColTester.result1[i] == ColTester.solution1 &&
                ColTester.result2[i] == ColTester.solution2) == true) ? AABB : -1;
            int sphereScore = ((ColTester.result3[i] == ColTester.solution3 &&
                ColTester.result4[i] == ColTester.solution4) == true) ? SPHERE : -1;
            int noErrorScore = (ColTester.errThrown[i] == false) ? noErr : 0;
            data += "\n" + student[i] + ", " + aabbScore.ToString() + ", ";
            data += ((aabbScore > 0) ? "false, " : "true, ") + sphereScore.ToString() + ", ";
            data += ((sphereScore > 0) ? "false, " : "true, ") + noErrorScore.ToString();
        }

        outputPath = Path.Combine(outputPath, "gradesP1.csv");
        File.WriteAllText(outputPath, data);

        csvRecorded = true;
        print("GRADES REPORTED, check for negative score for manual review!!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

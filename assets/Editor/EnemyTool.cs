using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;

public class EnemyTool : EditorWindow
{
    public List<Enemies> enemyList = new List<Enemies>();

    int myHealth, myAttack, myDefense, myAgility, myMana;
    float attackTime;

    bool isMagicUser;
    bool myToggle;
    bool myFold;
    bool nameFlag;
    bool spriteFlag;

    float minValue = 1;
    float maxValue = 20;
    float selectedMin = 1;
    float selectedMax = 10;

    string nameString ="";
    string[] enemyNames;
    List<string> enemyNameList = new List<string>();
    int currentChoice = 0;
    int lastChoice = 0;

    Sprite mySprite;

    [MenuItem("Custom Tools/ My Window %#j")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<EnemyTool>(true, "My Tool");
    }

    private void OnGUI()
    {
        currentChoice = EditorGUILayout.Popup(currentChoice, enemyNames);

        if (GUILayout.Button("Refresh Enemies"))
        {
            GetEnemies();
        }
        nameString = EditorGUILayout.TextField("Name: ", nameString);
        myHealth = EditorGUILayout.IntSlider("Health:", myHealth, 1, 300);
        myAttack = EditorGUILayout.IntSlider("Attack:", myAttack, 1, 100);
        myDefense = EditorGUILayout.IntSlider("Defense:", myDefense, 1, 100);
        myAgility = EditorGUILayout.IntSlider("Agility:", myAgility, 1, 100);
        attackTime = EditorGUILayout.Slider("Attack Time:", attackTime, minValue, maxValue);
        isMagicUser = EditorGUILayout.BeginToggleGroup("Is magic user?", isMagicUser);
        myMana = EditorGUILayout.IntSlider("Mana Pool:", myMana, 0, 100);
        EditorGUILayout.EndToggleGroup();

        
        if (nameFlag == true)
        {
            EditorGUILayout.HelpBox("Enemy must have a name to save", MessageType.Error);
        }

        mySprite = EditorGUILayout.ObjectField(mySprite, typeof(Sprite), true) as Sprite;
        if (mySprite != null)
        {
            Texture2D atext = SpriteUtility.GetSpriteTexture(mySprite, false);
            GUILayout.Label(atext);
        }

        if (spriteFlag)
        {
            EditorGUILayout.HelpBox("Enemy must have a sprite", MessageType.Error);
        }

        if (currentChoice == 0)
        {
            if (GUILayout.Button("Create"))
            {
                CreateEnemy();
            }
        }
        else
        {
            if (GUILayout.Button("Save"))
            {
                AlterEnemy();
            }
        }

        if (currentChoice != lastChoice)
        {
            if(currentChoice == 0)
            {
                //blank out fields for new enemy
                nameString = "";
                myHealth = 1;
                myAttack = 1;
                myDefense = 1;
                myAgility = 1;
                attackTime = 1;
                isMagicUser = false;
                myMana = 0;
                mySprite = null;
            }
            else
            {
                //load fields with enemy data
                nameString = enemyList[currentChoice - 1].emname;
                myHealth = enemyList[currentChoice - 1].health;
                myAttack = enemyList[currentChoice - 1].atk;
                myDefense = enemyList[currentChoice - 1].def;
                myAgility = enemyList[currentChoice - 1].agi;
                attackTime = enemyList[currentChoice - 1].atkTime;
                isMagicUser = enemyList[currentChoice - 1].isMagic;
                myMana = enemyList[currentChoice - 1].manaPool;
                mySprite = enemyList[currentChoice - 1].mySprite;
            }
            lastChoice = currentChoice;
        }
    }
    void Awake()
    {
        GetEnemies();
    }

    private void GetEnemies()
    {
        enemyList.Clear();
        enemyNameList.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Enemies");
        foreach(string guid in guids)
        {
            string myString = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(myString);

            Enemies enemyInst = AssetDatabase.LoadAssetAtPath(myString, typeof(Enemies)) as Enemies;

            enemyList.Add(enemyInst);
        }

        foreach (Enemies e in enemyList)
        {
            enemyNameList.Add(e.emname);
        }
        enemyNameList.Insert(0, "New");
        enemyNames = enemyNameList.ToArray();
    }

    private void CreateEnemy()
    {

        if(nameString == "")
        {
            nameFlag = true;
            return;
        }
        else if(mySprite == null)
        {
            spriteFlag = true;
            return;
        }
        else
        {
            Enemies newEnemy = ScriptableObject.CreateInstance<Enemies>();
            newEnemy.emname = nameString;
            newEnemy.health = myHealth;
            newEnemy.atk = myAttack;
            newEnemy.def = myDefense;
            newEnemy.agi = myAgility;
            newEnemy.atkTime = attackTime;
            newEnemy.isMagic = isMagicUser;
            newEnemy.manaPool = myMana;
            newEnemy.mySprite = mySprite;
            AssetDatabase.CreateAsset(newEnemy, "Assets/Resources/Data/EnemyData/" + newEnemy.emname.Replace(" ", "_") + ".asset");
            nameFlag = false;
            spriteFlag = false;
            GetEnemies();

            for(int i = 0; i < enemyNameList.Count; i++)
            {
                if(enemyNameList[i] == nameString)
                {
                    currentChoice = i;
                }
            }
        }
    }

    private void AlterEnemy()
    {
        if(nameString == "")
        {
            nameFlag = true;
            return;
        }
        else
        {
            enemyList[currentChoice - 1].emname = nameString;
            enemyList[currentChoice - 1].health = myHealth;
            enemyList[currentChoice - 1].atk = myAttack;
            enemyList[currentChoice - 1].def = myDefense;
            enemyList[currentChoice - 1].agi = myAgility;
            enemyList[currentChoice - 1].atkTime = attackTime;
            enemyList[currentChoice - 1].isMagic = isMagicUser;
            enemyList[currentChoice - 1].manaPool = myMana;
            enemyList[currentChoice - 1].mySprite = mySprite;
        }

        EditorUtility.SetDirty(enemyList[currentChoice - 1]);
        AssetDatabase.SaveAssets();
    }

}
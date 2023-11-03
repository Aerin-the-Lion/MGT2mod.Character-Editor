﻿using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;

namespace CharacterEditor
{
    public class CharacterSelectionMenu
    {

        static public characterScript personalCharacterScript;
        static public GameObject clonedCharacterObject;

        public static void SetClonedCharacter()
        {
            clonedCharacterObject = UnityEngine.Object.Instantiate(personalCharacterScript.gameObject);
            //クローン元と同じ位置情報を共有する
            clonedCharacterObject.transform.position = personalCharacterScript.gameObject.transform.position;
            clonedCharacterObject.transform.rotation = personalCharacterScript.gameObject.transform.rotation;
            clonedCharacterObject.transform.localScale = personalCharacterScript.gameObject.transform.localScale;
            clonedCharacterObject.name = "CHARNEWGAME";
        }

        /// <summary>
        /// 従業員メニュー -> 個人メニュー -> Character Editorを開くボタンを押したときに呼ばれる関数
        /// 個人のcharacterScriptをCharacter Editorに渡す
        /// </summary>
        public static void InitializeCharacterEditorWithSelectedCharacter()
        {
            Menu_NewGameCEO mainMenuScript = CharacterEditorManager.CharacterEditor.GetComponent<Menu_NewGameCEO>();
            Traverse.Create(mainMenuScript).Field("character").SetValue(null);
            Traverse.Create(mainMenuScript).Field("character").SetValue(clonedCharacterObject);

            //EditorのUIを初期化
            mainMenuScript.uiObjects[12].GetComponent<InputField>().text = "";  //.textを空にしないとInitしないので、あえて空にする
            mainMenuScript.Init();
        }

        /*
        /// <summary>
        /// 従業員メニューの個人メニューに、Character Editorを開くボタンを追加する関数
        /// in English: Add a button to open Character Editor to the personal menu of the employee menu
        /// </summary>
        static void SetSelectCharacterEditor_Button()
        {
            // 既に存在するselectCharacterEditor_Buttonを探す
            Transform parentTransform = GameObject.Find("CanvasInGameMenu").transform;
            Transform menu_Personal_ViewTransform = parentTransform.Find("Menu_PersonalView/WindowMain");
            Transform existingSelectCharacterEditor_ButtonTransform = menu_Personal_ViewTransform.Find("Button_Select_CharacterEditor");
            if (existingSelectCharacterEditor_ButtonTransform != null) { return; }            // selectCharacterEditor_Buttonが既に存在している場合、早期リターン

            //__instanceをクローン化
            GameObject menu_PersonalView = GameObject.Find("CanvasInGameMenu").transform.Find("Menu_PersonalView").gameObject;
            GameObject select_Button = menu_PersonalView.transform.Find("WindowMain").transform.Find("Button_Selektieren").gameObject;
            GameObject selectCharacterEditor_Button = UnityEngine.Object.Instantiate(select_Button);

            //クローン化したものの名前を変更
            selectCharacterEditor_Button.name = "Button_Select_CharacterEditor";
            //クローン化したものの親を変更
            selectCharacterEditor_Button.transform.SetParent(select_Button.transform.parent);
            //クローン化したものの位置を変更z
            selectCharacterEditor_Button.transform.localScale = select_Button.transform.localScale;
            selectCharacterEditor_Button.transform.localPosition = new Vector3(select_Button.transform.localPosition.x * 3.178f, 0, 0);

            //Buttonをクリックしたときの処理を追加
            //ボタンのコンポーネントを取得
            Button selectCharacterEditor_ButtonComponent = selectCharacterEditor_Button.GetComponent<Button>();
            //ボタンのコンポーネントのonClickを初期化
            selectCharacterEditor_ButtonComponent.onClick = new Button.ButtonClickedEvent();
            //ボタンのコンポーネントのonClickに処理を追加
            selectCharacterEditor_ButtonComponent.onClick.AddListener(cEBHandler_.OnSelectCharacterEditorButtonClicked);
        }
        */

        public static void Init()
        {
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            CharacterSelectionMenuButtonHandler.AddCharacterEditorButtonToPersonalMenu();
        }

        static bool IsNewGameCharacter(GameObject character) => character.name == "CHARNEWGAME";
        static bool IsCharacterEditor(GameObject me) => me.name == "Character Editor";

        public static Menu_NewGameCEO CharacterEditorInstance;
        public static GameObject createdCharacter;

        /// <summary>
        /// 既存のキャラクターデータをロードして、Character Editorに適用します。
        /// </summary>
        /// <param name="__instance">Menu_NewGameCEOのインスタンス(Character Editor)</param>
        /// <param name="___character">ロードするキャラクターオブジェクト</param>
        public static void ImportExistingCharacter(Menu_NewGameCEO __instance, GameObject ___character)
        {
            GameObject character = clonedCharacterObject;
            ___character = character;

            characterScript characterScript = character.GetComponent<characterScript>();

            // キャラクターの属性をCharacter Editorに適用
            __instance.uiObjects[12].GetComponent<InputField>().text = characterScript.myName;
            __instance.beruf = characterScript.beruf;
            __instance.s_skills = characterScript.GetBestSkillValue();
            __instance.s_gamedesign = characterScript.s_gamedesign;
            __instance.s_programmieren = characterScript.s_programmieren;
            __instance.s_grafik = characterScript.s_grafik;
            __instance.s_sound = characterScript.s_sound;
            __instance.s_pr = characterScript.s_pr;
            __instance.s_gametests = characterScript.s_gametests;
            __instance.s_technik = characterScript.s_technik;
            __instance.s_forschen = characterScript.s_forschen;


            //Perk処理
            for (int i = 0; i < __instance.perks.Length; i++)
            {
                if (characterScript.perks[i])
                {
                    __instance.perks[i] = true;
                }
                else
                {
                    __instance.perks[i] = false;
                }
            }

            if (characterScript.male)
            {
                __instance.male = true;
            }
            else
            {
                __instance.male = false;
            }

            __instance.body = characterScript.model_body;
            __instance.hair = characterScript.model_hair;
            __instance.eyes = characterScript.model_eyes;
            __instance.beard = characterScript.model_beard;
            __instance.colorSkin = characterScript.model_skinColor;
            __instance.colorHair = characterScript.model_hairColor;
            __instance.colorShirt = characterScript.model_ShirtColor;
            __instance.colorHose = characterScript.model_HoseColor;
            __instance.colorAdd1 = characterScript.model_Add1Color;


            CharacterEditorInstance = __instance;
            createdCharacter = ___character;
        }

            // --------------------- 以下、Harmonyのパッチ関数 ---------------------


            /// <summary>
            /// 既存のキャラクターデータをロードして、Character Editorに適用します。
            /// </summary>
            /// <param name="__instance">Menu_NewGameCEOのインスタンス(Character Editor)</param>
            /// <param name="___character">ロードするキャラクターオブジェクト</param>
            /// <returns>通常の処理(CEOの作成の場合は、trueを返し、CharacterEditorの場合は、falseを返す</returns>
            [HarmonyPrefix, HarmonyLib.HarmonyPatch(typeof(Menu_NewGameCEO), "LoadData")]
        public static bool OnLoadDataMenu_NewGameCEO(Menu_NewGameCEO __instance, GameObject ___character)
        {
            // Character Customization Editorの条件を確認
            if (!IsCharacterEditor(__instance.gameObject)) { return true; }

            // キャラクターの属性をCharacter Editorに適用
            ImportExistingCharacter(__instance, ___character);
            return false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class LuaMainUIContext : IUIContext
    {
        public string Name => "Main";

        public string AssetPath => $"Assets/Game/UI/Canvas-Main-XLua.prefab";

        public bool Multiple => false;

        public IUIContext Clone()
        {
            return this;
        }
    }

    public partial class PreloadState
    {
        public override void OnEnter(FSM<GameStateContext> fsm)
        {
            base.OnEnter(fsm);

        //    GameMode.Event.AddListener<LoadDataTableEventArgs>(OnLoadDataTable);

             Debug.Log("PreloadState");
            
            // GameMode.DataTable.LoadDataTable("Assets/Game/DataTable/GameCheckpoint.txt");
            //  GameMode.DataTable.LoadDataTable("Assets/Game/DataTable/SmallGamesGradeTwo.txt");
            // TextAsset txt = await GameMode.Resource.LoadAsset<TextAsset>("Assets/Game/DataTable/fdsf.txt");
            //  Debug.Log(txt.text);
            //  GameObject.Find("Cube").GetComponent<MeshRenderer>().material=await GameMode.Resource.LoadAsset<Material>("Assets/Game/Materials/New Material 4.mat");
            //  GameMode.Resource.LoadSceneAsync("Assets/Game/Scenes/scene002.unity",UnityEngine.SceneManagement.LoadSceneMode.Additive);

            //GameMode.UI.Push(new MainUIContext(), (data) =>
            //{
            //	TestUIAnimation ta = new TestUIAnimation();
            //             MainUIAnimation ma = new MainUIAnimation();

            //             GameMode.UI.Push(new TestUIContext())
            //	.OnAnimationStart((lastUIView, nextUIView) => {  })
            //	.OnAnimationComplete((lastUIView, nextUIView) => { 
            //                 Debug.Log("Animation Complete");
            //                 lastUIView.gameObject.SetActive(false);
            //             })
            //	.OnUITweenReady((lastUIView, nextUIView) =>
            //	{
            //                 Debug.Log("Animation Ready");
            //                 nextUIView.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1920, 0f);
            //                 lastUIView.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0f);
            //                 ta.TargetUIView = nextUIView;
            //                 ma.TargetUIView = lastUIView;
            //                 ta.SetTarget(nextUIView.gameObject);
            //                 ma.SetTarget(lastUIView.gameObject);
            //             })
            //	.SetAnimations(new IUIAnimation[] { ma, ta })
            //             .RunAnimation(false);
            //});

            GameMode.XLua.DoMainLua();

            GameMode.UI.Push(new LuaMainUIContext());
        }
        public override void OnExit(FSM<GameStateContext> fsm)
        {
            GameMode.Event.RemoveListener<LoadDataTableEventArgs>(OnLoadDataTable);

            base.OnExit(fsm);
        }

        public override void OnInit(FSM<GameStateContext> fsm)
        {
            base.OnInit(fsm);
        }

        public override void OnUpdate(FSM<GameStateContext> fsm)
        {
            base.OnUpdate(fsm);
        }


        private void OnLoadDataTable(object sender,IEventArgs e)
        {
            LoadDataTableEventArgs ne = e as LoadDataTableEventArgs;
            if(ne!=null)
            {
               IDataTable idt = ne.Data;

               TableData td=idt[20010014]["UIFormId"];
               int uiFormId = (int)td;
                
               Debug.Log($"#################################:{ne.Message}");
               foreach (var item in idt)
               {
                   Debug.Log($"-------------------------------------------------------");
                   TableData td02 = idt[item];
                   foreach (var item02 in td02)
                   {
                       Debug.Log(item02.ToString());
                   }
               }
            }
        }

      
    }
}


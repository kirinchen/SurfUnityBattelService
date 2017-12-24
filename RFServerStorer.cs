using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet {
    public class RFServerStorer : MonoBehaviour {

        public static readonly string KEY_LOAD_CONFIG = "RFNEet.load.name";
        public static readonly string DEFAULT_CONFIG = "RFServerStorer";

        private static RFServerStorer instance;
        private List<URestApi> uApis = new List<URestApi>();
        public string scoreCalcClassName = "RFNEet.DefaultPingScoreCalc";
        public string pingDtoFinder = "RFNEet.PingDtoFinder";
        private PingBetter currentPingBetter;


        private PingDtoFinder newPingFinder() {
            PingDto.ScoreCalc calc = CommUtils.newInstance<PingDto.ScoreCalc>(scoreCalcClassName);
            PingDtoFinder ans = CommUtils.newInstance<PingDtoFinder>(pingDtoFinder);
            ans.init(calc);
            return ans;
        }

        private void init() {
            injectUaBundles();
            DontDestroyOnLoad(gameObject);
        }

        private void injectUaBundles() {
            URestApi[] uapis = GetComponentsInChildren<URestApi>();
            foreach (URestApi api in uapis) {
                if (api.enabled) {
                    uApis.Add(api);
                }
            }
        }

        public PingBetter genPingBetter() {
            if (currentPingBetter != null) {
                currentPingBetter.terminal();
                currentPingBetter = null;
            }
            currentPingBetter = new PingBetter(newPingFinder(), this, listAll());
            return currentPingBetter;
        }

        public List<URestApi> listAll() {
            return uApis;
        }


        public static RFServerStorer getInstance() {
            if (instance == null) {
                instance = createInstance();
                instance.init();
            }
            return instance;
        }

        public static RFServerStorer createInstance() {
            ConstantRepo cr = ConstantRepo.getInstance();
            string rn = cr.opt<string>(KEY_LOAD_CONFIG, null);
            rn = string.IsNullOrEmpty(rn) ? DEFAULT_CONFIG : rn;
            RFServerStorer temp = Resources.Load<RFServerStorer>(rn);
            return Instantiate(temp);
        }
    }
}

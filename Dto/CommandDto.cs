using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RFNEet {
    public class CommandDto {

        public enum Command {
            HIDE, UN_HIDE
        }

        public Command cmd;

        public CommandDto() { }

        public CommandDto(Command cmd) {
            this.cmd = cmd;
        }
    }
}

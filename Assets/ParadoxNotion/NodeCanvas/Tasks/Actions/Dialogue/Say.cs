﻿using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
    [Category("Dialogue")]
    [Description(
        "You can use a variable inline with the text by using brackets likeso: [myVarName] or [Global/myVarName].\nThe bracket will be replaced with the variable value ToString")]
    [Icon("Dialogue")]
    public class Say : ActionTask<IDialogueActor>
    {
        public Statement statement = new("This is a dialogue text...");

        protected override string info => string.Format("<i>' {0} '</i>", statement.text.CapLength(30));

        protected override void OnExecute()
        {
            var tempStatement = statement.BlackboardReplace(blackboard);
            DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(agent, tempStatement, EndAction));
        }
    }
}
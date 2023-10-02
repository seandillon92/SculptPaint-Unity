using System;
using System.Collections.Generic;
using UnityEditorInternal;

internal class RenderDocCapture
{
    static Dictionary<Action, int> m_counter = new Dictionary<Action, int>();
    static internal bool RunWithCapture(Action action, int max)
    {
        if (!m_counter.ContainsKey(action))
        {
            m_counter.Add(action, 0);
        }

        m_counter[action]++;
        var counter= m_counter[action];

        if (counter> max)
        {
            action();
            return false;
        }

        var game = UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.GameView"));
        RenderDoc.BeginCaptureRenderDoc(game);
        action();
        RenderDoc.EndCaptureRenderDoc(game);

        return true;
    }
}

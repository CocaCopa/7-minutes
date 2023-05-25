using System;

public enum Yokai_State {
    LibraryEvent,
}

[Serializable]
public class YokaiStates {

    private static Yokai_State currentState;

    public static Yokai_State GetYokaiState() {

        return currentState;
    }

    public static void SetYokaiState(Yokai_State state) {

        currentState = state;
    }
}

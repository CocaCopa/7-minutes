using System;

public enum State {
    GameStart,
    LibraryEvent,
    FollowBehindPlayer,
    WaittingForAction
}

public static class YokaiStates {

    private static State currentState;

    public static State GetYokaiState() {

        return currentState;
    }

    public static void SetYokaiState(State state) {

        currentState = state;
    }
}

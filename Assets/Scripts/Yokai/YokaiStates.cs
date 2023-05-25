public static class YokaiStates {

    public enum State {
        GameStart,
        LibraryEvent,
        FollowBehindPlayer,
        WaittingForAction
    }

    private static State currentState;

    public static State GetYokaiState() {

        return currentState;
    }

    public static void SetYokaiState(State state) {

        currentState = state;
    }
}

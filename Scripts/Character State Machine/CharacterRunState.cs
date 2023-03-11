public class CharacterRunState : CharacterBaseState{
    public CharacterRunState(CharacterStateMachine currentContext, CharacterStateFactory characterStateFactory) :
        base(currentContext, characterStateFactory){
    }
    public override void EnterState(){
        throw new System.NotImplementedException();
    }

    public override void UpdateState(){
        throw new System.NotImplementedException();
    }

    public override void ExitState(){
        throw new System.NotImplementedException();
    }

    public override void CheckSwitchStates(){
        throw new System.NotImplementedException();
    }

    public override void InitializeSubState(){
        throw new System.NotImplementedException();
    }
}
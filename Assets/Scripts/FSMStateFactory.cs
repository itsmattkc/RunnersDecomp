public class FSMStateFactory<Context>
{
	public int stateID;

	public FSMState<Context> state;

	public FSMStateFactory(int id, FSMState<Context> st)
	{
		stateID = id;
		state = st;
	}
}

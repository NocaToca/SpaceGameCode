using Godot;
using System;

public partial class BaseController : Node2D
{
	private CharacterBody2D main_node {get{return GetNode<CharacterBody2D>("CharacterBody2D");}}
	private Sprite2D character {get{return main_node.GetNode<Sprite2D>("Character");}} 

	//Basically, on input, we're going to assess where we're clicking and then move our little icon accordingly.
	//Keep in mind this game is going to be a top down, so we're going to actually do a click-based movement
	
	/*
		-First make basic movement (Just go directly to target)
		-Make fluid movement (Travel time, movement speed)
		-Make idea for smart movment (avoids obstacles, goes the quickest route) - Wow! NavigationServer seems very powerful. Let us implement that
		-Refine smart movement
	*/

	//I'm not pressed to make anything, this is all just to learn Godot. The game itself is not a pressing matter either,
	//Since I'm still on the design and worldbuilding process.

	//Tween for animation
	Tween movement_tween;
	float movement_speed = 25.0f; //(m/s)
	private Vector2 destination_vector; 

	//Okay, now we have the Tween, let's add the navigation mesh!
	private NavigationAgent2D navigation_agent;
	private bool started = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		//AddChild(movement_tween);
		//We need to initialize the agent here!
		navigation_agent = main_node.GetNode<NavigationAgent2D>("NavigationAgent2D");
		destination_vector = character.Position;

		navigation_agent.PathDesiredDistance = 4.0f;
		navigation_agent.TargetDesiredDistance = 4.0f;

		Callable.From(ActorSetup).CallDeferred();
	}

	//We need to wait for the physic frame to set up to sync
	private async void ActorSetup(){
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

		started = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta){
	}

	//I don't know why they do it in physics process instead of process (Maybe to do it before colliders?)
	//Actually I think it's because NavigationAgent2D.GetNextPosition() runs on the physics threads
	public override void _PhysicsProcess(double delta){
		if(!started || navigation_agent.IsNavigationFinished()){
			return; //We don't want to do anything if we're at the destination!
		}

		MoveCharacterNavMesh(navigation_agent.GetNextPathPosition());

	}

	//Moves the character relative to the nav mesh!
	private void MoveCharacterNavMesh(Vector2 to_position){
		Vector2 velocity = (to_position - character.GlobalTransform.Origin).Normalized();
		velocity *= movement_speed;
		main_node.Velocity = velocity;
		main_node.MoveAndSlide();
	}

	//Called for inputs
	public override void _Input(InputEvent @event){

		//First, we just check if the event is related to a mouse button
		if(@event is InputEventMouseButton mouse_button_event){
			if(mouse_button_event.Pressed && mouse_button_event.ButtonIndex == MouseButton.Right){
				//Right button has been clicked
				SetDestination(mouse_button_event.GlobalPosition);

			}
		}
	}

	public void SetDestination(Vector2 destination){
		destination_vector = destination;
		navigation_agent.TargetPosition = destination;
	}

	//Uses our movement speed to grab the amount of time we need
	public float GetTimeToMove(Vector2 start, Vector2 end){
		//Essentially we just grab the distance then divide by time (easy!)
		float distance = start.DistanceTo(end);
		return distance/movement_speed;
	}

	public void MoveCharacter(Vector2 to_position){
		//character.Position = global_position; //Wow! But this snaps to a position, not allowing us to have fluid movement :c
		if(movement_tween != null){
			movement_tween.Kill();
		}
		//Something I read about were Tweens, so let's try to use that. Apparently they're great for interpolation!
		movement_tween = GetTree().CreateTween();

		movement_tween.TweenProperty(character, "position", to_position, 1.0f);


	}
}

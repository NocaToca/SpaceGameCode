using Godot;
using System;

public partial class BaseController : Node2D
{

	private Sprite2D character {get{return GetNode<Sprite2D>("Character");}} 

	//Basically, on input, we're going to assess where we're clicking and then move our little icon accordingly.
	//Keep in mind this game is going to be a top down, so we're going to actually do a click-based movement
	
	/*
		-First make basic movement (Just go directly to target)
		-Make fluid movement (Travel time, movement speed)
		-Make idea for smart movment (avoids obstacles, goes the quickest route)
		-Refine smart movement
	*/

	//I'm not pressed to make anything, this is all just to learn Godot. The game itself is not a pressing matter either,
	//Since I'm still on the design and worldbuilding process.

	//Tween for animation
	Tween movement_tween;
	float movement_speed = 0.5f; //(m/s)

	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		//AddChild(movement_tween);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//Called for inputs
	public override void _Input(InputEvent @event){

		//First, we just check if the event is related to a mouse button
		if(@event is InputEventMouseButton mouse_button_event){
			if(mouse_button_event.Pressed && mouse_button_event.ButtonIndex == MouseButton.Right){
				//Right button has been clicked
				MoveCharacter(mouse_button_event.GlobalPosition);

			}
		}
	}

	//Uses our movement speed to grab the amount of time we need
	public float GetTimeToMove(Vector2 start, Vector2 end){
		//Essentially we just grab the distance then divide by time (easy!)
		float distance = start.DistanceTo(end);
		return distance/movement_speed;
	}

	public void MoveCharacter(Vector2 global_position){
		//character.Position = global_position; //Wow! But this snaps to a position, not allowing us to have fluid movement :c
		if(movement_tween != null){
			movement_tween.Kill();
		}
		//Something I read about were Tweens, so let's try to use that. Apparently they're great for interpolation!
		movement_tween = GetTree().CreateTween();

		movement_tween.TweenProperty(character, "position", global_position, 1.0f);


	}
}

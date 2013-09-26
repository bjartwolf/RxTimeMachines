<Query Kind="Program">
  <NuGetReference>Rx-Main</NuGetReference>
  <NuGetReference>Rx-Testing</NuGetReference>
  <Namespace>Microsoft.Reactive.Testing</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
</Query>

void Main()
{
	
	var present = new TestScheduler();
	var future = new TestScheduler();
	// We'll give the future som headstart from the present
	 future.AdvanceBy(TimeSpan.FromDays(10).Ticks); 
	
	var cassandra = new Cassandra();
	var trojans = new Trojans(cassandra);
	var horse = new Horse(present, trojans);
	cassandra.Trojans = trojans;
	
	var futureCassandra = new Cassandra();
	futureCassandra.Self = cassandra;
	var futuresTrojans = new Trojans(futureCassandra);
	var futuresHorse = new Horse(future, futuresTrojans);
	futureCassandra.Trojans = futuresTrojans;
	
	for (int i = 0; i < 700; i++) {
		future.AdvanceBy(TimeSpan.FromDays(1).Ticks);
		present.AdvanceBy(TimeSpan.FromDays(1).Ticks);	
	}
	present.AdvanceBy(TimeSpan.FromDays(10).Ticks);
	if (future.Now == present.Now) {
		"Present and future are now in sync".Dump();
		if (futuresTrojans.Alive != trojans.Alive) {
			"But we are both dead and alive. This is a paradox!".Dump();
		} else {
			"Time and space is still in order".Dump();
		}
	}
}

class Trojans {
	public bool Alive {get;set;}
	private bool Skeptics = false; // Skeptics ignore warnings from the future
	public bool BeenWarned {get;set;}
	private Cassandra Cassandra {get;set;} 
	public Trojans(Cassandra cassandra) {
		Alive = true;
		BeenWarned = false;
		Cassandra = cassandra;
	}
	
	private void OpenDoors() {
		"Trojans: What a nice horse. It must be a present. Open the gates".Dump();
		Alive = false;
		Cassandra.RapeAndAbduct();
	}
	
	public void Knock() {
		if (BeenWarned) {
			"Trojans: We have been warned".Dump();
			if (Skeptics) {
				"But we don't listen to that crazy women!".Dump();
				OpenDoors();
			} else {
				"Trojans: We have been warned from the future. We know your cunning plan!".Dump();
			}
		} else {
			OpenDoors();
		}
	}
}	

class Horse
{
	private Trojans Trojans {get;set;}
	public IScheduler Scheduler {get;set;}
	public Horse(IScheduler sched, Trojans trojans) {
		Scheduler = sched;
		Trojans = trojans;
		// Stand outside the gates at given absolute time in history
		Observable.Timer((new DateTime(2, 1, 2)),Scheduler).Subscribe((_) => {
				"Horse: Look at us, nice present, yes?".Dump();
				Trojans.Knock();
			});
		Observable.Timer((new DateTime(2, 11, 20)),Scheduler).Subscribe((_) => {
				"A few months passed, demonstrating that time is moving side-by-side".Dump();
			});

	}
}

class Cassandra 
{
	public Cassandra Self {get;set;}
	public Trojans Trojans {get;set;}

	public void Warn() {
		"Cassandra: You must never open for the horse. It's a trap".Dump();
		Trojans.BeenWarned = true;
	}
	
	public void RapeAndAbduct () {
		"Cassandra: Ajax the Lesser, you stink!".Dump();
		if (Self != null) {
			"I should warn myself!".Dump();
			Self.Warn();
		}
	}
}
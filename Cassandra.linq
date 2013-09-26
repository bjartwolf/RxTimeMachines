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
	// http://en.wikipedia.org/wiki/Cassandra
	// Cassandra has two timelines.
	// One in which she will listen to events in the future (destruction of Troy)
	// The Trojans, if they believe her, will not let the trojan horse into Troy
	// and therefore avoid destruction of their city. If they do believe her, they will not allow the horse in
	
	var present = new TestScheduler();
	var future = new TestScheduler();
	// We'll give the future som headstart from the present
	 future.AdvanceBy(TimeSpan.FromDays(10).Ticks); 
	
	var cassandra = new Cassandra(present, null);
	var trojans = new Trojans(present, cassandra);
	var horse = new Horse(present, trojans);
	cassandra.Trojans = trojans;
	
	var futureCassandra = new Cassandra(future, cassandra);
	var futuresTrojans = new Trojans(future, futureCassandra);
	var futuresHorse = new Horse(future, futuresTrojans);
	futureCassandra.Trojans = futuresTrojans;
	
	future.AdvanceBy(TimeSpan.FromDays(700).Ticks);
	present.AdvanceBy(TimeSpan.FromDays(710).Ticks);
	if (future.Now == present.Now) {
		"We are at the same time".Dump();
		if (futuresTrojans.Alive != trojans.Alive) {
			"But we are both dead and alive. This is a paradox!".Dump();
		} else {
			"Time and space is still in order".Dump();
		}
	}
	
}
class Trojans {
	public IScheduler Scheduler {get;set;}
	public bool Alive {get;set;}
	public bool Skeptics {get;set;} // Skeptics ignore warnings from the future
	public bool BeenWarned {get;set;}
	public Cassandra Cassandra {get;set;} 
	public Trojans(IScheduler sched, Cassandra cassandra) {
		Alive = true;
		BeenWarned = false;
		Skeptics = false;
		Scheduler = sched;
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
	public IScheduler Scheduler {get;set;}
	public Trojans Trojans {get;set;}
	
	public Horse(IScheduler sched, Trojans trojans) {
		Scheduler = sched;
		Trojans = trojans;
		// Stand outside the gates at given absolute time in history
		Observable.Timer((new DateTime(2, 1, 2)),Scheduler).Subscribe(
			(_) => {
				"Horse: Look at us, nice present, yes?".Dump();
				Trojans.Knock();
			});
	}
}

class Cassandra 
{
	public TestScheduler Scheduler {get;set;}
	public Cassandra Self {get;set;}
	public Trojans Trojans {get;set;}
	public Cassandra(TestScheduler sched, Cassandra self) {
		Scheduler = sched;
		Self = self;
	}	

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
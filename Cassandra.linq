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
	
	var cassandra = new Cassandra(present);
	var trojans = new Trojans(present, cassandra);
	var horse = new Horse(present, trojans);

	var futureCassandra = new Cassandra(future);
	var futuresTrojans = new Trojans(future, futureCassandra);
	var futuresHorse = new Horse(future, futuresTrojans);
	
	present.AdvanceBy(TimeSpan.FromDays(710).Ticks);
	future.AdvanceBy(TimeSpan.FromDays(700).Ticks);
	if (future.Now == present.Now) {
		"We are at the same time".Dump();
		if (futuresTrojans.Alive != trojans.Alive) {
			"This is a paradox!".Dump();
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
		Skeptics = true;
		Scheduler = sched;
		Cassandra = cassandra;
	}
	
	public void Knock() {
		if (!Skeptics && BeenWarned) {
			"Trojans: We have been warned from the future. We know your cunning plan!".Dump();
		} else {
			"Trojans: What a nice horse. It must be a present. Open the gates".Dump();
			Alive = false;
			Cassandra.RapeAndAbduct();
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
		// Stand outside the gates
		Observable.Timer((new DateTime(2, 1, 2)),Scheduler).Subscribe(
			(_) => {
				("Time " + Scheduler.Now).Dump();
				"Horse: Look at us, nice present, yes?".Dump();
				Trojans.Knock();
			});
	}
}

class Cassandra 
{
	public TestScheduler Scheduler {get;set;}
	
	public Cassandra(TestScheduler sched) {
		Scheduler = sched;
	}	
	
	public void RapeAndAbduct () {
		"Cassandra: Ajax the Lesser, you stink!".Dump();
	}
}
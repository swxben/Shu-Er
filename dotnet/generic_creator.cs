/*
 * Run this in LinqPad. You just get "I AM FOO" twice, but it is how
 * foo2 is created that is interesting.
 * 
 * This shows how to create a generic creator, which could be used for
 * dependency injection, where a consumer receives a function that is
 * used to create the injected type. GetCreator<T>() is straightforward,
 * but GetCreator<T1, T>() (or GetCreator<Tx, ... T1, T>()) is more 
 * interesting as it lets you pass arguments to the injected type's
 * constructor. Note that this makes the injected constructor a part 
 * of the contract that can't be captured at compile type.
 * 
 * This doesn't demonstrate DI , in a real-world example the 
 * 'where T: new()' constraints would have to be removed and something 
 * like late binding or reflection used to create the injected type.
 * 
 * */


class Foo { public string ToString() { return "I AM FOO"; } }

static T Get<T>() where T: new() { return new T(); }
static T Get<T, T1>(T1 ignored) where T: new() { return new T(); }

static Func<T> GetCreator<T>() where T: new() { return Get<T>; }
static Func<T1, T> GetCreator<T, T1>() where T: new() { return Get<T, T1>; }


void Main()
{
	var fooCreator = GetCreator<Foo>();
	var foo = fooCreator();	
	foo.ToString().Dump();

	var fooCreator2 = GetCreator<Foo, string>();
	var foo2 = fooCreator2("ignored");
	foo2.ToString().Dump();
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGameMechanicsTest {
    class Program {
        static void Main(string[] args) {
            (new Game(5)).Start();
        }
    }

    public class Game {

        public static Person Spy;
        public static Person[] people;
        public static List<TraitProfile> DocumentedTraitProfiles;

        public Game(int amountOfPeople) {
            people = new Person[amountOfPeople];
            for (int i = 0; i < amountOfPeople; i++) {
                people[i] = new Person("");
            };
            MakeSomeoneSpy();
            DocumentedTraitProfiles = new List<TraitProfile>();
        }

        public static bool documentTrait(Person person, Trait trait) {
            bool profileExists = false;
            TraitProfile personsProfile = null;
            foreach (TraitProfile profile in DocumentedTraitProfiles) {
                if (profile.ProfileTarget == person) {
                    personsProfile = profile;
                    profileExists = true;
                };
            }
            if (!profileExists) {
                personsProfile = new TraitProfile(person);
                DocumentedTraitProfiles.Add(personsProfile);
            }
            return personsProfile.NotateTrait(trait);
        }

        public void Start() {
            while (true) {
                Console.Clear();
                InterrogateAPerson();
                Console.Clear();
                AccuseAPerson();
            }
        }

        public void MakeSomeoneSpy() {
            Game.Spy = people[Dice.Roll(people.Length - 1)];
            Game.Spy.Profession = Profession.SPY;
        }

        public void InterrogateAPerson() {
            Console.WriteLine("INTERROGATION FAZE");
            Console.WriteLine("Select A person From 0-" + (people.Length - 1) + " to interrogate");
            int idx = int.Parse(Console.ReadLine());
            people[idx].presentATraitOfYours();
            people[idx].presentATraitOfTheSpy();
        }

        public void AccuseAPerson() {
            Console.WriteLine("ACCUSSE FAZE");
            int idx = 0;
            foreach (Person person in people) {
                foreach (TraitProfile profile in DocumentedTraitProfiles) {
                    if (profile.ProfileTarget == person && person.Traits.Count > 0) {
                        Console.WriteLine(person.GetFullIdentity()+":");
                        foreach (Trait trait in profile.KnownTraits) {
                            Console.WriteLine("\t" + trait.GetStatement() + "\n");
                        }
                    }
                }
                idx++;
            }
            Console.WriteLine("Would you like to accuse a person? (Y/N) \n!!![You will lose if you guess wrong!]");
            char input;
            do {
                input = Console.ReadKey().KeyChar;
            } while (input != 'n' && input != 'y');
            if (input == 'y') {
                Console.WriteLine("Select A person From 0-" + (people.Length - 1) + " to accusse");
                int inputIdx = int.Parse(Console.ReadLine());
                if (people[inputIdx] == Game.Spy) {
                    Console.WriteLine("YOU FOUND THE SPY!");
                }
                else {
                    Console.WriteLine("YOU FAILED TO FIND SPY..");
                }
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }

    public static class Dice {
        static Random random = new Random();
        public static int Roll(int max) {
            return random.Next(max);
        }
    }

    public class TraitProfile {
        public Person ProfileTarget;
        public List<Trait> KnownTraits;
        public TraitProfile(Person profileTarget) {
            this.ProfileTarget = profileTarget;
            this.KnownTraits = new List<Trait>();
        }
        public bool NotateTrait(Trait newTrait) {
            foreach (Trait knownTrait in KnownTraits) {
                if (newTrait == knownTrait) {
                    return false;
                }
            }
            KnownTraits.Add(newTrait);
            return true;
        }
    }

    public class Person {

        public string Name;
        public int ID;
        public Profession Profession;

        private static int idCount = 0;
        private static int IdCount { get { int id = idCount; idCount++; return id; } set { idCount = value; } }
        public List<Trait> Traits;

        public Person() {
            this.initialize();
        }

        public Person(string name) {
            initialize();
            this.Name = name;
        }

        private void initialize() {
            this.Name = "";
            this.ID = IdCount;
            this.Traits = new List<Trait>();
            this.Profession = Profession.MAILMAN;
            GenerateTraits(4);
        }

        public void GenerateTraits(int amount) {
            for (int i = 0; i < amount; i++) {
                Traits.Add(Trait.GenerateRandomTrait());
            }
        }

        public string GetFullIdentity() {
            return ID + ":" + Name;
        }
        public void presentATraitOfYours() {
            string identity = GetFullIdentity();
            Trait trait = Traits[Dice.Roll(Traits.Count - 1)];
            string statement = trait.GetStatement();
            Console.WriteLine(identity + ": I " + statement);
            Console.ReadKey();
            Game.documentTrait(this, trait);
        }

        public void presentATraitOfTheSpy() {
            string identity = GetFullIdentity();
            Trait trait = Game.Spy.Traits[Dice.Roll(Traits.Count - 1)];
            string statement = trait.GetStatement();
            Console.WriteLine(identity + ": I heard the Spy " + statement);
            Console.ReadKey();
        }
    }

    public enum Profession {
        SPY, MAILMAN, MILKMAN
    }

    public class Trait {
        public Relationship Relationship;
        public Subject Subject;
        public Adverb Adverb;

        public Trait(Relationship relationship, Subject subject, Adverb adverb) {
            this.Relationship = relationship;
            this.Subject = subject;
            this.Adverb = adverb;
        }

        public string GetStatement() {
            return Relationship.ToString() + " " + Adverb.ToString() + " " + Subject.ToString() + "S";
        }

        public static Trait GenerateRandomTrait() {
            Array values = Enum.GetValues(typeof(Relationship));
            Relationship relationship = (Relationship)values.GetValue(Dice.Roll(values.Length));
            values = Enum.GetValues(typeof(Subject));
            Subject subject = (Subject)values.GetValue(Dice.Roll(values.Length));
            values = Enum.GetValues(typeof(Adverb));
            Adverb adverb = (Adverb)values.GetValue(Dice.Roll(values.Length));
            return new Trait(relationship, subject, adverb);
        }
    }

    public enum Relationship {
        LIKES, DISLIKES
    }

    public enum Adverb {
        RED, GREEN, BLUE, YELLOW
    }

    public enum Subject {
        TILE, PEOPLE, HAT, SHOE, SHIRT
    }

}

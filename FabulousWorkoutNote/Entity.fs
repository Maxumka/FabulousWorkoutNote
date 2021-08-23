namespace FabulousWorkoutNote

open System
open System.Collections.Generic
open System.ComponentModel.DataAnnotations.Schema;
open Microsoft.EntityFrameworkCore

module Entity = 
    [<AllowNullLiteral>]
    type WorkoutEntity() = 
        [<DatabaseGenerated(DatabaseGeneratedOption.Identity)>]
        member val Id = 0 with get, set
        member val Date = DateTime() with get, set
        member val Exercises = new List<ExerciseEntity>() with get, set
    and ExerciseEntity() = 
        [<DatabaseGenerated(DatabaseGeneratedOption.Identity)>]
        member val Id = 0 with get, set
        member val Name = "" with get, set
        member val Sets = new List<SetEntity>() with get, set
        member val WorkoutId = 0 with get, set
        member val Workout = new WorkoutEntity() with get, set 
    and SetEntity() = 
        [<DatabaseGenerated(DatabaseGeneratedOption.Identity)>]
        member val Id = 0 with get, set
        member val Weight = 0 with get, set
        member val Reps = 0 with get, set
        member val ExerciseId = 0 with get, set
        member val Exercise = new ExerciseEntity() with get, set
    
module Context = 
    open Entity

    type AppDbContext (dbPath : string) = 
        inherit DbContext()

        member public _.GetPath = dbPath

        [<DefaultValue>]
        val mutable sets : DbSet<SetEntity>
        member public this.Sets with get () = this.sets 
                                and set s = this.sets <- s

        [<DefaultValue>]
        val mutable exercises : DbSet<ExerciseEntity>
        member public this.Exercises with get () = this.exercises
                                     and set e = this.exercises <- e

        [<DefaultValue>]
        val mutable workouts : DbSet<WorkoutEntity>
        member public this.Workouts with get () = this.workouts 
                                    and set w = this.workouts <- w

        override __.OnConfiguring(optionsBuilder : DbContextOptionsBuilder) =
            optionsBuilder.UseSqlite($"Filename={dbPath};") |> ignore

        override __.OnModelCreating(modelBuilder : ModelBuilder) = 
            modelBuilder.Entity<WorkoutEntity>().ToTable("Workout") |> ignore
            modelBuilder.Entity<WorkoutEntity>().HasKey("Id") |> ignore
            modelBuilder.Entity<WorkoutEntity>().Property("Date") |> ignore
            
            modelBuilder.Entity<ExerciseEntity>().ToTable("Exercise") |> ignore
            modelBuilder.Entity<ExerciseEntity>().HasKey("Id") |> ignore
            modelBuilder.Entity<ExerciseEntity>().Property("Name") |> ignore
            modelBuilder.Entity<ExerciseEntity>().HasOne("Workout")
                                                 .WithMany("Exercises")
                                                 .HasForeignKey("WorkoutId") |> ignore

            modelBuilder.Entity<SetEntity>().ToTable("Set") |> ignore
            modelBuilder.Entity<SetEntity>().HasKey("Id") |> ignore
            modelBuilder.Entity<SetEntity>().Property("Weight") |> ignore
            modelBuilder.Entity<SetEntity>().Property("Reps") |> ignore
            modelBuilder.Entity<SetEntity>().HasOne("Exercise")
                                            .WithMany("Sets")
                                            .HasForeignKey("ExerciseId") |> ignore        
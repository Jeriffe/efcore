// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace Microsoft.EntityFrameworkCore.ChangeTracking
{
    public class SkipCollectionEntryTest
    {
        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_back_reference(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var entity = new Cherry();
            context.Add(entity);

            var entityEntry = context.Entry(entity);
            Assert.Same(entityEntry.Entity, entityEntry.Collection("Chunkies").EntityEntry.Entity);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_back_reference_generic(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var entity = new Cherry();
            context.Add(entity);

            var entityEntry = context.Entry(entity);
            Assert.Same(entityEntry.Entity, entityEntry.Collection(e => e.Chunkies).EntityEntry.Entity);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_metadata(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var entity = new Cherry();
            context.Add(entity);

            Assert.Equal("Chunkies", context.Entry(entity).Collection("Chunkies").Metadata.Name);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_metadata_generic(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var entity = new Cherry();
            context.Add(entity);

            Assert.Equal("Chunkies", context.Entry(entity).Collection(e => e.Chunkies).Metadata.Name);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_and_set_current_value(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry = new Cherry();
            var chunky = new Chunky();
            context.AddRange(chunky, cherry);

            var collection = context.Entry(cherry).Collection("Chunkies");
            var inverseCollection = context.Entry(chunky).Collection("Cherries");

            Assert.Null(collection.CurrentValue);

            collection.CurrentValue = new List<Chunky> { chunky };

            Assert.Same(chunky, cherry.Chunkies.Single());
            Assert.Same(cherry, chunky.Cherries.Single());
            Assert.Same(chunky, collection.CurrentValue.Cast<Chunky>().Single());
            Assert.Same(cherry, inverseCollection.CurrentValue.Cast<Cherry>().Single());
            Assert.Same(collection.FindEntry(chunky).GetInfrastructure(), context.Entry(chunky).GetInfrastructure());

            collection.CurrentValue = null;

            Assert.Empty(chunky.Cherries);
            Assert.Null(cherry.Chunkies);
            Assert.Null(collection.CurrentValue);
            Assert.Empty(inverseCollection.CurrentValue);
            Assert.Null(collection.FindEntry(chunky));
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_and_set_current_value_generic(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry = new Cherry();
            var chunky = new Chunky();
            context.AddRange(chunky, cherry);

            var collection = context.Entry(cherry).Collection(e => e.Chunkies);
            var inverseCollection = context.Entry(chunky).Collection(e => e.Cherries);

            Assert.Null(collection.CurrentValue);

            collection.CurrentValue = new List<Chunky> { chunky };

            Assert.Same(chunky, cherry.Chunkies.Single());
            Assert.Same(cherry, chunky.Cherries.Single());
            Assert.Same(chunky, collection.CurrentValue.Single());
            Assert.Same(cherry, inverseCollection.CurrentValue.Single());
            Assert.Same(collection.FindEntry(chunky).GetInfrastructure(), context.Entry(chunky).GetInfrastructure());

            collection.CurrentValue = null;

            Assert.Empty(chunky.Cherries);
            Assert.Null(cherry.Chunkies);
            Assert.Null(collection.CurrentValue);
            Assert.Empty(inverseCollection.CurrentValue);
            Assert.Null(collection.FindEntry(chunky));
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_and_set_current_value_not_tracked(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry = new Cherry();
            var chunky = new Chunky();

            var collection = context.Entry(cherry).Collection("Chunkies");
            var inverseCollection = context.Entry(chunky).Collection("Cherries");

            Assert.Null(collection.CurrentValue);

            collection.CurrentValue = new List<Chunky> { chunky };

            Assert.Same(chunky, cherry.Chunkies.Single());
            Assert.Null(chunky.Cherries);
            Assert.Same(chunky, collection.CurrentValue.Cast<Chunky>().Single());
            Assert.Null(inverseCollection.CurrentValue);

            collection.CurrentValue = null;

            Assert.Null(chunky.Cherries);
            Assert.Null(cherry.Chunkies);
            Assert.Null(collection.CurrentValue);
            Assert.Null(inverseCollection.CurrentValue);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_and_set_current_value_generic_not_tracked(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry = new Cherry();
            var chunky = new Chunky();

            var collection = context.Entry(cherry).Collection(e => e.Chunkies);
            var inverseCollection = context.Entry(chunky).Collection(e => e.Cherries);

            Assert.Null(collection.CurrentValue);

            collection.CurrentValue = new List<Chunky> { chunky };

            Assert.Same(chunky, cherry.Chunkies.Single());
            Assert.Null(chunky.Cherries);
            Assert.Same(chunky, collection.CurrentValue.Single());
            Assert.Null(inverseCollection.CurrentValue);

            collection.CurrentValue = null;

            Assert.Null(chunky.Cherries);
            Assert.Null(cherry.Chunkies);
            Assert.Null(collection.CurrentValue);
            Assert.Null(inverseCollection.CurrentValue);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_and_set_current_value_start_tracking(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry = new Cherry();
            var chunky = new Chunky();
            context.Add(cherry);

            var collection = context.Entry(cherry).Collection("Chunkies");
            var inverseCollection = context.Entry(chunky).Collection("Cherries");

            Assert.Null(collection.CurrentValue);

            collection.CurrentValue = new List<Chunky> { chunky };

            Assert.Same(chunky, cherry.Chunkies.Single());
            Assert.Same(cherry, chunky.Cherries.Single());
            Assert.Same(chunky, collection.CurrentValue.Cast<Chunky>().Single());
            Assert.Same(cherry, inverseCollection.CurrentValue.Cast<Cherry>().Single());

            Assert.Equal(EntityState.Added, context.Entry(cherry).State);
            Assert.Equal(EntityState.Added, context.Entry(chunky).State);

            collection.CurrentValue = null;

            Assert.Empty(chunky.Cherries);
            Assert.Null(cherry.Chunkies);
            Assert.Null(collection.CurrentValue);
            Assert.Empty(inverseCollection.CurrentValue);

            Assert.Equal(EntityState.Added, context.Entry(cherry).State);
            Assert.Equal(EntityState.Added, context.Entry(chunky).State);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_get_and_set_current_value_start_tracking_generic(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry = new Cherry();
            var chunky = new Chunky();
            context.Add(cherry);

            var collection = context.Entry(cherry).Collection(e => e.Chunkies);
            var inverseCollection = context.Entry(chunky).Collection(e => e.Cherries);

            Assert.Null(collection.CurrentValue);

            collection.CurrentValue = new List<Chunky> { chunky };

            Assert.Same(chunky, cherry.Chunkies.Single());
            Assert.Same(cherry, chunky.Cherries.Single());
            Assert.Same(chunky, collection.CurrentValue.Single());
            Assert.Same(cherry, inverseCollection.CurrentValue.Single());

            Assert.Equal(EntityState.Added, context.Entry(cherry).State);
            Assert.Equal(EntityState.Added, context.Entry(chunky).State);

            collection.CurrentValue = null;

            Assert.Empty(chunky.Cherries);
            Assert.Null(cherry.Chunkies);
            Assert.Null(collection.CurrentValue);
            Assert.Empty(inverseCollection.CurrentValue);

            Assert.Equal(EntityState.Added, context.Entry(cherry).State);
            Assert.Equal(EntityState.Added, context.Entry(chunky).State);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsModified_tracks_detects_deletion_of_related_entity(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry1 = new Cherry { Id = 1 };
            var cherry2 = new Cherry { Id = 2 };
            var chunky1 = new Chunky { Id = 1 };
            var chunky2 = new Chunky { Id = 2 };

            cherry1.Chunkies = new List<Chunky> { chunky1, chunky2 };
            context.AttachRange(cherry1, chunky2, chunky1, chunky2);

            var relatedToCherry1 = context.Entry(cherry1).Collection(e => e.Chunkies);
            var relatedToCherry2 = context.Entry(cherry2).Collection(e => e.Chunkies);
            var relatedToChunky1 = context.Entry(chunky1).Collection(e => e.Cherries);
            var relatedToChunky2 = context.Entry(chunky2).Collection(e => e.Cherries);

            Assert.False(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.False(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);

            context.Entry(chunky1).State = EntityState.Deleted;

            Assert.True(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.True(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);

            context.Entry(chunky1).State = EntityState.Unchanged;

            Assert.False(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.False(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsModified_tracks_adding_new_related_entity(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry1 = new Cherry { Id = 1 };
            var cherry2 = new Cherry { Id = 2 };
            var chunky1 = new Chunky { Id = 1 };
            var chunky2 = new Chunky { Id = 2 };

            cherry1.Chunkies = new List<Chunky> { chunky1, chunky2 };
            context.AttachRange(cherry1, chunky2, chunky1, chunky2);

            var relatedToCherry1 = context.Entry(cherry1).Collection(e => e.Chunkies);
            var relatedToCherry2 = context.Entry(cherry2).Collection(e => e.Chunkies);
            var relatedToChunky1 = context.Entry(chunky1).Collection(e => e.Cherries);
            var relatedToChunky2 = context.Entry(chunky2).Collection(e => e.Cherries);

            var chunky3 = new Chunky { Id = 3 };
            cherry1.Chunkies.Add(chunky3);
            context.ChangeTracker.DetectChanges();

            Assert.True(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.False(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);

            context.Entry(chunky3).State = EntityState.Detached;

            Assert.False(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.False(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsModified_tracks_removing_items_from_the_join_table(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry1 = new Cherry { Id = 1 };
            var cherry2 = new Cherry { Id = 2 };
            var chunky1 = new Chunky { Id = 1 };
            var chunky2 = new Chunky { Id = 2 };

            cherry1.Chunkies = new List<Chunky> { chunky1, chunky2 };
            context.AttachRange(cherry1, chunky2, chunky1, chunky2);

            var relatedToCherry1 = context.Entry(cherry1).Collection(e => e.Chunkies);
            var relatedToCherry2 = context.Entry(cherry2).Collection(e => e.Chunkies);
            var relatedToChunky1 = context.Entry(chunky1).Collection(e => e.Cherries);
            var relatedToChunky2 = context.Entry(chunky2).Collection(e => e.Cherries);

            cherry1.Chunkies.Remove(chunky2);
            context.ChangeTracker.DetectChanges();

            Assert.True(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.False(relatedToChunky1.IsModified);
            Assert.True(relatedToChunky2.IsModified);

            cherry1.Chunkies.Add(chunky1);

            Assert.False(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.False(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsModified_tracks_adding_items_to_the_join_table(bool useExplicitPk)
        {
            using var context = useExplicitPk ? new ExplicitFreezerContext() : new FreezerContext();

            var cherry1 = new Cherry { Id = 1 };
            var cherry2 = new Cherry { Id = 2 };
            var chunky1 = new Chunky { Id = 1 };
            var chunky2 = new Chunky { Id = 2 };

            cherry1.Chunkies = new List<Chunky> { chunky1, chunky2 };
            cherry2.Chunkies = new List<Chunky>();
            context.AttachRange(cherry1, chunky2, chunky1, chunky2);

            var relatedToCherry1 = context.Entry(cherry1).Collection(e => e.Chunkies);
            var relatedToCherry2 = context.Entry(cherry2).Collection(e => e.Chunkies);
            var relatedToChunky1 = context.Entry(chunky1).Collection(e => e.Cherries);
            var relatedToChunky2 = context.Entry(chunky2).Collection(e => e.Cherries);

            cherry2.Chunkies.Add(chunky1);
            context.ChangeTracker.DetectChanges();

            Assert.False(relatedToCherry1.IsModified);
            Assert.True(relatedToCherry2.IsModified);
            Assert.True(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);

            cherry2.Chunkies.Remove(chunky1);

            Assert.False(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.False(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);
        }

        [ConditionalFact]
        public void IsModified_tracks_mutation_of_join_fks()
        {
            using var context = new ExplicitFreezerContext();

            var joinEntity = context.Set<Dictionary<string, object>>("CherryChunky").Add(new Dictionary<string, object>());
            var iee = joinEntity.GetInfrastructure();
            var property = iee.EntityType.FindProperty("Id");

            Assert.Equal(0, iee[property]);
            Assert.True(iee.HasDefaultValue(property));



            Assert.Equal(0, joinEntity.Property<int>("Id").CurrentValue);


            var cherry1 = new Cherry { Id = 1 };
            var cherry2 = new Cherry { Id = 2 };
            var chunky1 = new Chunky { Id = 1 };
            var chunky2 = new Chunky { Id = 2 };

            cherry1.Chunkies = new List<Chunky> { chunky1, chunky2 };
            cherry2.Chunkies = new List<Chunky>();
            context.AddRange(cherry1, chunky2, chunky1, chunky2);
            context.ChangeTracker.Entries().ToList().ForEach(e => e.State = EntityState.Unchanged);

            var relatedToCherry1 = context.Entry(cherry1).Collection(e => e.Chunkies);
            var relatedToCherry2 = context.Entry(cherry2).Collection(e => e.Chunkies);
            var relatedToChunky1 = context.Entry(chunky1).Collection(e => e.Cherries);
            var relatedToChunky2 = context.Entry(chunky2).Collection(e => e.Cherries);

            //var joinEntity = context.Set<Dictionary<string, object>>("CherryChunky").Find(1);

            cherry2.Chunkies.Add(chunky1);
            context.ChangeTracker.DetectChanges();

            Assert.False(relatedToCherry1.IsModified);
            Assert.True(relatedToCherry2.IsModified);
            Assert.True(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);

            cherry2.Chunkies.Remove(chunky1);

            Assert.False(relatedToCherry1.IsModified);
            Assert.False(relatedToCherry2.IsModified);
            Assert.False(relatedToChunky1.IsModified);
            Assert.False(relatedToChunky2.IsModified);
        }

        [ConditionalTheory]
        [InlineData(EntityState.Detached, EntityState.Added)]
        [InlineData(EntityState.Added, EntityState.Added)]
        [InlineData(EntityState.Modified, EntityState.Added)]
        [InlineData(EntityState.Deleted, EntityState.Added)]
        [InlineData(EntityState.Unchanged, EntityState.Added)]
        [InlineData(EntityState.Detached, EntityState.Deleted)]
        [InlineData(EntityState.Added, EntityState.Deleted)]
        [InlineData(EntityState.Modified, EntityState.Deleted)]
        [InlineData(EntityState.Deleted, EntityState.Deleted)]
        [InlineData(EntityState.Unchanged, EntityState.Deleted)]
        public void IsModified_can_set_fk_to_modified_principal_with_Added_or_Deleted_dependents(
            EntityState principalState, EntityState dependentState)
        {
            using var context = new FreezerContext();

            var cherry = new Cherry();
            var chunky1 = new Chunky { Id = 1 };
            var chunky2 = new Chunky { Id = 2 };
            cherry.Chunkies = new List<Chunky> { chunky1, chunky2 };

            context.Entry(cherry).State = principalState;
            context.Entry(chunky1).State = dependentState;
            context.Entry(chunky2).State = dependentState;

            var collection = context.Entry(cherry).Collection(e => e.Chunkies);
            var inverseCollection1 = context.Entry(chunky1).Collection(e => e.Cherries);
            var inverseCollection2 = context.Entry(chunky2).Collection(e => e.Cherries);

            var principalIsModified = principalState == EntityState.Added || principalState == EntityState.Deleted;

            Assert.True(collection.IsModified);
            Assert.Equal(principalIsModified, inverseCollection1.IsModified);
            Assert.Equal(principalIsModified, inverseCollection2.IsModified);

            collection.IsModified = false;

            Assert.True(collection.IsModified);

            collection.IsModified = true;

            Assert.True(collection.IsModified);
            Assert.Equal(principalIsModified, inverseCollection1.IsModified);
            Assert.Equal(principalIsModified, inverseCollection2.IsModified);
            Assert.Equal(dependentState, context.Entry(chunky1).State);
            Assert.Equal(dependentState, context.Entry(chunky2).State);

            if (dependentState == EntityState.Deleted)
            {
                context.Entry(chunky1).State = EntityState.Detached;
                context.Entry(chunky2).State = EntityState.Detached;
            }
            else
            {
                context.Entry(chunky1).State = EntityState.Unchanged;
                context.Entry(chunky2).State = EntityState.Unchanged;
            }

            Assert.False(collection.IsModified);
            Assert.Equal(principalIsModified, inverseCollection1.IsModified);
            Assert.Equal(principalIsModified, inverseCollection2.IsModified);
        }

        private class Chunky
        {
            public int Id { get; set; }
            public ICollection<Cherry> Cherries { get; set; }
        }

        private class Cherry
        {
            public int Id { get; set; }
            public ICollection<Chunky> Chunkies { get; set; }
        }

        private class FreezerContext : DbContext
        {
            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseInternalServiceProvider(InMemoryFixture.DefaultServiceProvider)
                    .UseInMemoryDatabase(nameof(FreezerContext));

            public DbSet<Chunky> Icecream { get; set; }
        }

        private class ExplicitFreezerContext : FreezerContext
        {
            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseInternalServiceProvider(InMemoryFixture.DefaultServiceProvider)
                    .UseInMemoryDatabase(nameof(ExplicitFreezerContext));

            protected internal override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder
                    .Entity<Cherry>().HasMany(e => e.Chunkies).WithMany(e => e.Cherries)
                    .UsingEntity<Dictionary<string, object>>(
                        "CherryChunky",
                        b => b.HasOne<Chunky>().WithMany().HasForeignKey("ChunkyId"),
                        b => b.HasOne<Cherry>().WithMany().HasForeignKey("CherryId"))
                    .IndexerProperty<int>("Id");
            }
        }
    }
}

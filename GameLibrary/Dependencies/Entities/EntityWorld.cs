using System;
using System.Collections.Generic;
using GameLibrary.Dependencies.Physics.Dynamics;
using Microsoft.Xna.Framework;
namespace GameLibrary.Dependencies.Entities
{
    public class EntityWorld : PhysicsWorld{
        private SystemManager systemManager;
        private EntityManager entityManager;
        private TagManager tagManager;
        private GroupManager groupManager;
        private Bag<Entity> refreshed = new Bag<Entity>();
        private Bag<Entity> deleted = new Bag<Entity>();        
        private Dictionary<String,Stack<int>> cached = new Dictionary<String, Stack<int>>();
        private Dictionary<String, IEntityTemplate> entityTemplates = new Dictionary<String, IEntityTemplate>();
        private Dictionary<String, IEntityGroupTemplate> entityGroupTemplates = new Dictionary<String, IEntityGroupTemplate>();
        private int delta;
        
        public EntityWorld(Vector2 Gravity) : base(Gravity) 
        {
            entityManager = new EntityManager(this);
            systemManager = new SystemManager(this);
            tagManager = new TagManager(this);
            groupManager = new GroupManager(this);        
        }
        
        public GroupManager GroupManager {
            get { return groupManager; }
        }
        
        public SystemManager SystemManager {
            get { return systemManager; }
        }
        
        public EntityManager EntityManager {
            get { return entityManager; }
        }
        
        public TagManager TagManager {
            get { return tagManager; }
        }
        
        /**
         * Time since last game loop.
         * @return delta in milliseconds.
         */
        public int Delta {
            get { return delta; }
            set { delta = value; }
        }

        /// <summary>
        /// Update time on all of the systems.
        /// </summary>
        public float EntitySystemUpdateTime
        {
            get;
            internal set;
        }
        
        /**
         * Delete the provided entity from the world.
         * @param e entity
         */
        public void DeleteEntity(Entity e) {
            System.Diagnostics.Debug.Assert(e != null);
            deleted.Add(e);
        }
        
        /**
         * Ensure all systems are notified of changes to this entity.
         * @param e entity
         */
        internal void RefreshEntity(Entity e) {
            System.Diagnostics.Debug.Assert(e != null);
            refreshed.Add(e);
        }

           
        /**
         * Create and return a new or reused entity instance.
         * @return entity
         */
        public Entity CreateEntity() {
            return entityManager.Create();
        }
        
        public Entity CreateEntity(string entityTemplateTag, params object[] templateArgs) {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(entityTemplateTag));
            Entity e = entityManager.Create();  
            IEntityTemplate entityTemplate;
            entityTemplates.TryGetValue(entityTemplateTag, out entityTemplate);
            return entityTemplate.BuildEntity(e, templateArgs);
        }

        public Entity[] CreateEntityGroup(string entityGroupTemplateTag, string entityGroupName, params object[] templateArgs)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(entityGroupTemplateTag));
            IEntityGroupTemplate entityGroupTemplate;
            entityGroupTemplates.TryGetValue(entityGroupTemplateTag, out entityGroupTemplate);
            Entity[] entityGroup = entityGroupTemplate.BuildEntityGroup(this, templateArgs);
            //Give them a group:
            foreach (Entity e in entityGroup)
            {
                e.Group = entityGroupName;
                e.Refresh();
            }
            //Return dat shit
            return entityGroup;
            
        }

        public void SetEntityTemplate(string entityTemplateTag, IEntityTemplate entityTemplate)
        {
            entityTemplates.Add(entityTemplateTag, entityTemplate);
        }

        public void SetEntityGroupTemplate(string entityGroupTemplateTag, IEntityGroupTemplate entityGroupTemplate)
        {
            entityGroupTemplates.Add(entityGroupTemplateTag, entityGroupTemplate);
        }
        
        /**
         * Get a entity having the specified id.
         * @param entityId
         * @return entity
         */
        public Entity GetEntity(int entityId) {
            System.Diagnostics.Debug.Assert(entityId >= 0);
            return entityManager.GetEntity(entityId);
        }


        public void LoopStart()
        {
            if (!deleted.IsEmpty)
            {
                for (int i = 0, j = deleted.Size; j > i; i++)
                {
                    Entity e = deleted.Get(i);
                    entityManager.Remove(e);
                    groupManager.Remove(e);
                    if( e != null)
                        e.DeletingState = false;
                   
                }
                deleted.Clear();
            }

            if (!refreshed.IsEmpty)
            {
                for (int i = 0, j = refreshed.Size; j > i; i++)
                {
                    Entity e = refreshed.Get(i);
                    entityManager.Refresh(e);
                    e.RefreshingState = false;
                }
                refreshed.Clear();
            }
        }
        
        public Dictionary<Entity,Bag<Component>> CurrentState {
            get
            {
                Bag<Entity> entities = entityManager.ActiveEntities;
                Dictionary<Entity, Bag<Component>> currentState = new Dictionary<Entity, Bag<Component>>();
                for (int i = 0, j = entities.Size; i < j; i++)
                {
                    Entity e = entities.Get(i);
                    Bag<Component> components = e.Components;
                    currentState.Add(e, components);
                }
                return currentState;
            }
        }

        /// <summary>
        /// Loads the state of the entity.
        /// </summary>
        /// <param name="templateTag">The template tag. Can be null</param>
        /// <param name="groupName">Name of the group. Can be null</param>
        /// <param name="components">The components.</param>
        /// <param name="templateArgs">Params for entity template</param>
        public void LoadEntityState(String templateTag, String groupName,Bag<Component> components, params object[] templateArgs) {
            System.Diagnostics.Debug.Assert(components != null);
            Entity e;
            if(!String.IsNullOrEmpty(templateTag)) {
                e = CreateEntity(templateTag, templateArgs);
            } else {
                e = CreateEntity();
            }
            if (String.IsNullOrEmpty(groupName))
            {
                groupManager.Set(groupName,e);
            }        
            for(int i = 0, j = components.Size; i < j; i++) {
                e.AddComponent(components.Get(i));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace SopraProjekt.Entities.AI
{
    public class Tremaux
    {
        private readonly Random mRandom;

        public Tremaux()
        {
            mRandom = new Random();
        }

        public void FindNextPoint(Hero entity)
        {
            // Markiere Ausgangspfad
            if (entity.AiLastWayPoint.mMarks[entity][entity.AiDirection] < 2)
            {
                entity.AiLastWayPoint.mMarks[entity][entity.AiDirection] += 1;
            }
            // Markiere Eingangspfad
            if (entity.AiCurrentWayPoint.mMarks[entity][GetOppositeDirection(entity.AiDirection)] < 2)
            {
                entity.AiCurrentWayPoint.mMarks[entity][GetOppositeDirection(entity.AiDirection)] += 1;
            }

            if (entity.AiCurrentWayPoint.mNumberNeighbours == 1)
            {
                // Turn around at dead end
                TurnAround(entity);
                return;
            }

            if (entity.AiCurrentWayPoint.mNumberNeighbours == 2)
            {
                // continue walk at path, if path is no dead end and no crossing
                var next = GetMinNeighbour(entity.AiCurrentWayPoint, entity);
                entity.AiLastWayPoint = entity.AiCurrentWayPoint;
                entity.AiCurrentWayPoint = next.Item2;
                entity.AiDirection = next.Item1;
                return;
            }
            
            if (entity.AiCurrentWayPoint.mNumberNeighbours >= 3)
            {
                // Wenn Eingangspfad mit 2 markiert ist, dann zufällig aus min wählen
                if (entity.AiCurrentWayPoint.mMarks[entity][GetOppositeDirection(entity.AiDirection)] == 2)
                {
                    var neighbours = GetMinNeighbour(entity.AiCurrentWayPoint, entity);
                    entity.AiLastWayPoint = entity.AiCurrentWayPoint;
                    entity.AiCurrentWayPoint = neighbours.Item2;
                    entity.AiDirection = neighbours.Item1;
                    return;
                }

                // Wenn Eingangspfad jetzt mit eins markiert
                if (entity.AiCurrentWayPoint.mMarks[entity][GetOppositeDirection(entity.AiDirection)] == 1)
                {
                    // keine andere markierung: zufällig (nicht zurück)
                    int sum = 0;
                    for (int i=0; i < 4; i++)
                    {
                        // Dont use marks that marked with value 5 (wall)
                        if (entity.AiCurrentWayPoint.mMarks[entity][i] != 5)
                        {
                            sum += entity.AiCurrentWayPoint.mMarks[entity][i];
                        }
                    }

                    if (sum == 1)
                    {
                        var neighbours = GetMinNeighbour(entity.AiCurrentWayPoint, entity);
                        entity.AiLastWayPoint = entity.AiCurrentWayPoint;
                        entity.AiCurrentWayPoint = neighbours.Item2;
                        entity.AiDirection = neighbours.Item1;
                    }
                    else
                    {
                        // sonst zurück
                        TurnAround(entity);
                    }
                }
            }
        }

        private Tuple<int, WayPoint> GetMinNeighbour(WayPoint point, Hero hero)
        {
            var list = new List<Tuple<int, WayPoint>>();
            var min = point.mMarks[hero].Min();
            for (var i = 0; i < 4; i++)
            {
                if (point.mMarks[hero][i] == min)
                {
                    list.Add(new Tuple<int, WayPoint>(i, point.mNeighbours[i]));
                }
            }

            return list[mRandom.Next(0, list.Count)];
        }

        private void TurnAround(Hero entity)
        {
            var next = entity.AiLastWayPoint;
            entity.AiLastWayPoint = entity.AiCurrentWayPoint;
            entity.AiCurrentWayPoint = next;
            entity.AiDirection = GetOppositeDirection(entity.AiDirection);
        }

        private int GetOppositeDirection(int direction)
        {
            return (direction + 2) % 4;
        }
    }
}
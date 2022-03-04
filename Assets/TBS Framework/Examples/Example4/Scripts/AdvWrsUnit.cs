using System.Collections;
using System.Collections.Generic;
using TbsFramework.Cells;
using TbsFramework.Units;
using UnityEngine;

namespace TbsFramework.Example4
{
    public class AdvWrsUnit : Unit
    {
        public string UnitName;

        public override void Initialize()
        {
            base.Initialize();
            transform.localPosition += new Vector3(0, 0, -0.1f);
        }

        public override void MarkAsAttacking(Unit other)
        {
            StartCoroutine(AttackAnimation(other));
        }

        public override void MarkAsDefending(Unit other)
        {
            StartCoroutine(DefenceAnimation());
        }

        public override void MarkAsDestroyed()
        {
        }

        public override void MarkAsFinished()
        {
            SetColor(new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        public override void MarkAsFriendly()
        {
            SetColor(new Color(0, 1, 0, 0.25f));
        }

        public override void MarkAsReachableEnemy()
        {
            SetColor(new Color(1, 0, 0, 0.5f));
        }

        public override void MarkAsSelected()
        {
        }

        public override void UnMark()
        {
            SetColor(new Color(0, 0, 0, 0));
        }

        protected override int Defend(Unit other, int damage)
        {
            return damage - (Cell as AdvWrsSquare).DefenceBoost;
        }

        public override void Move(Cell destinationCell, List<Cell> path)
        {
            GetComponent<SpriteRenderer>().sortingOrder += 10;
            transform.Find("marker").GetComponent<SpriteRenderer>().sortingOrder += 10;
            transform.Find("mask").GetComponent<SpriteRenderer>().sortingOrder += 10;
            base.Move(destinationCell, path);
        }

        protected override void OnMoveFinished()
        {
            GetComponent<SpriteRenderer>().sortingOrder -= 10;
            transform.Find("marker").GetComponent<SpriteRenderer>().sortingOrder -= 10;
            transform.Find("mask").GetComponent<SpriteRenderer>().sortingOrder -= 10;
            base.OnMoveFinished();
        }

        public override bool IsCellTraversable(Cell cell)
        {
            return base.IsCellTraversable(cell) || (cell.CurrentUnit != null && cell.CurrentUnit.PlayerNumber == PlayerNumber);
        }

        private void SetColor(Color color)
        {
            var highlighter = transform.Find("marker");
            var spriteRenderer = highlighter.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }

        private IEnumerator AttackAnimation(Unit other)
        {
            var heading = other.transform.position - transform.position;
            var direction = heading / heading.magnitude;
            float startTime = Time.time;

            while (startTime + 0.25f > Time.time)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + (direction / 5f), ((startTime + 0.25f) - Time.time));
                yield return 0;
            }
            startTime = Time.time;
            while (startTime + 0.25f > Time.time)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position - (direction / 5f), ((startTime + 0.25f) - Time.time));
                yield return 0;
            }
            transform.position = Cell.transform.position + new Vector3(0, 0, -0.1f);
        }

        private IEnumerator DefenceAnimation()
        {
            var rnd = new System.Random();
            
            for(int i=0; i<5; i++)
            {
                var heading = new Vector3((float)rnd.NextDouble() - 0.5f, (float)rnd.NextDouble() - 0.5f, 0);
                var direction = heading / heading.magnitude;
                float startTime = Time.time;

                while (startTime + 0.05f > Time.time)
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + direction, ((startTime + 0.05f) - Time.time));
                    yield return 0;
                }
                startTime = Time.time;
                while (startTime + 0.05f > Time.time)
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position - direction, ((startTime + 0.05f) - Time.time));
                    yield return 0;
                }
                transform.position = Cell.transform.position + new Vector3(0, 0, -0.1f);
            }
        }
    }
}

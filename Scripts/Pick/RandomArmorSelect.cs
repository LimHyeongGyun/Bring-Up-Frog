using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace Pickup
{
    //����ġ �����̱�
    public class WeightedRandomPicker<T>
    {
        //��ü �������� ����ġ ��
        public double SumOfWeights
        {
            get
            {
                CalculateSumIfDirty();
                return _sumOfWeights;
            }
        }

        private System.Random randomInstance;
        private readonly Dictionary<T, double> itemWeightDict;
        private readonly Dictionary<T, double> normalizedItemWeightDict;

        //����ġ ���� ������ ���� �������� ����
        private bool isDirty;
        private double _sumOfWeights;


        public WeightedRandomPicker()
        {
            randomInstance = new System.Random();
            itemWeightDict = new Dictionary<T, double>();
            normalizedItemWeightDict = new Dictionary<T, double>();
            isDirty = true;
            _sumOfWeights = 0.0;
        }

        public WeightedRandomPicker(int randomSeed)
        {
            randomInstance = new System.Random(randomSeed);
            itemWeightDict = new Dictionary<T, double>();
            normalizedItemWeightDict = new Dictionary<T, double>();
            isDirty = true;
            _sumOfWeights = 0.0;
        }

        //���ο� ������ - ����ġ �� �߰�
        public void Add(T item, double weight)
        {
            CheckDuplicatedItem(item);
            CheckValidWeight(weight);

            itemWeightDict.Add(item, weight);
            isDirty = true;
        }
        //���ο� ������-����ġ �ֵ� �߰�
        public void Add(params (T item, double weight)[] pairs)
        {
            foreach (var pair in pairs)
            {
                CheckDuplicatedItem(pair.item);
                CheckValidWeight(pair.weight);

                itemWeightDict.Add(pair.item, pair.weight);
            }
            isDirty = true;
        }

        /************************************************
         *                 Public Methods                  
         ************************************************/

        //��Ͽ��� ��� ������ ����
        public void Remove(T item)
        {
            CheckNotExistedItem(item);

            itemWeightDict.Remove(item);
            isDirty = true;
        }

        //��� �������� ����ġ ����
        public void ModifyWeight(T item, double weight)
        {
            CheckNotExistedItem(item);
            CheckValidWeight(weight);

            itemWeightDict[item] = weight;
            isDirty = true;
        }

        //���� �õ� �缳��
        public void ReSeed(int seed)
        {
            randomInstance = new System.Random(seed);
        }

        //���� �̱�
        public T GetRandomPick()
        {
            // ���� ���
            double chance = randomInstance.NextDouble(); // [0.0, 1.0)
            chance *= SumOfWeights;

            return GetRandomPick(chance);
        }

        //���� ���� ���� �����Ͽ� �̱�
        public T GetRandomPick(double randomValue)
        {
            if (randomValue < 0.0) randomValue = 0.0;
            if (randomValue > SumOfWeights) randomValue = SumOfWeights - 0.00000001;

            double current = 0.0;
            foreach (var pair in itemWeightDict)
            {
                current += pair.Value;

                if (randomValue < current)
                {
                    return pair.Key;
                }
            }

            throw new Exception($"Unreachable - [Random Value : {randomValue}, Current Value : {current}]");
        }

        //��� �������� ����ġ Ȯ��
        public double GetWeight(T item)
        {
            return itemWeightDict[item];
        }

        //��� �������� ����ȭ�� ����ġ Ȯ��
        public double GetNormalizedWeight(T item)
        {
            CalculateSumIfDirty();
            return normalizedItemWeightDict[item];
        }

        //������ ��� Ȯ��(�б� ����)
        public ReadOnlyDictionary<T, double> GetItemDictReadonly()
        {
            return new ReadOnlyDictionary<T, double>(itemWeightDict);
        }

        //����ġ ���� 1�� �ǵ��� ����ȭ�� ������ ��� Ȯ��(�б� ����)
        public ReadOnlyDictionary<T, double> GetNormalizedItemDictReadonly()
        {
            CalculateSumIfDirty();
            return new ReadOnlyDictionary<T, double>(normalizedItemWeightDict);
        }

        /// <summary> ��� �������� ����ġ �� ����س��� </summary>
        private void CalculateSumIfDirty()
        {
            if (!isDirty) return;
            isDirty = false;

            _sumOfWeights = 0.0;
            foreach (var pair in itemWeightDict)
            {
                _sumOfWeights += pair.Value;
            }

            // ����ȭ ��ųʸ��� ������Ʈ
            UpdateNormalizedDict();
        }

        /// <summary> ����ȭ�� ��ųʸ� ������Ʈ </summary>
        private void UpdateNormalizedDict()
        {
            normalizedItemWeightDict.Clear();
            foreach (var pair in itemWeightDict)
            {
                normalizedItemWeightDict.Add(pair.Key, pair.Value / _sumOfWeights);
            }
        }

        /// <summary> �̹� �������� �����ϴ��� ���� �˻� </summary>
        private void CheckDuplicatedItem(T item)
        {
            if (itemWeightDict.ContainsKey(item))
                throw new Exception($"�̹� [{item}] �������� �����մϴ�.");
        }

        /// <summary> �������� �ʴ� �������� ��� </summary>
        private void CheckNotExistedItem(T item)
        {
            if (!itemWeightDict.ContainsKey(item))
                throw new Exception($"[{item}] �������� ��Ͽ� �������� �ʽ��ϴ�.");
        }

        /// <summary> ����ġ �� ���� �˻�(0���� Ŀ�� ��) </summary>
        private void CheckValidWeight(in double weight)
        {
            if (weight <= 0f)
                throw new Exception("����ġ ���� 0���� Ŀ�� �մϴ�.");
        }
    }
}

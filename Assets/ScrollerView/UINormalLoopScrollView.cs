using MGame.General;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CircularScrollView
{
    /// <summary>
    /// 常用循环列表
    /// </summary>
    public class UINormalLoopScrollView : UICircularScrollView
    {
        private GridLayoutGroup m_GridContent;
        private string m_OnClickNodeName;
        private Dictionary<GameObject, bool> isAddedListener = new Dictionary<GameObject, bool>();
        private GameObject m_GoTemplate;

        public void Init(DelegateGoAndIdx callBack, DelegateGoAndIdx onClickCallBack, string goOnClickNode)
        {
            DisposeAll();

            m_FuncCallBackFunc = callBack;
            m_OnClickNodeName = goOnClickNode;

            if (onClickCallBack != null)
            {
                m_FuncOnClickCallBack = onClickCallBack;
            }

            if (m_isInited)
                return;

            m_Content = GetComponent<ScrollRect>().content.gameObject;
            m_GridContent = GetComponent<ScrollRect>().content.GetComponent<GridLayoutGroup>();
            m_Row = m_GridContent.constraintCount;

            if (m_CellGameObject == null)
            {
                m_CellGameObject = m_Content.transform.GetChild(0).gameObject;
            }

            m_GoTemplate = Instantiate(m_CellGameObject);

            /* Cell 处理 */
            //m_CellGameObject.transform.SetParent(m_Content.transform.parent, false);
            SetPoolsObj(m_GoTemplate);

            RectTransform cellRectTrans = m_GoTemplate.GetComponent<RectTransform>();
            cellRectTrans.pivot = new Vector2(0f, 1f);
            CheckAnchor(cellRectTrans);
            cellRectTrans.anchoredPosition = Vector2.zero;

            //记录 Cell 信息
            m_CellObjectHeight = m_GridContent.cellSize.y;//cellRectTrans.rect.height;
            m_CellObjectWidth = m_GridContent.cellSize.x; //cellRectTrans.rect.width;

            //记录 Plane 信息
            rectTrans = GetComponent<RectTransform>();
            Rect planeRect = rectTrans.rect;
            m_PlaneHeight = planeRect.height;
            m_PlaneWidth = planeRect.width;

            //记录 Content 信息
            m_ContentRectTrans = m_Content.GetComponent<RectTransform>();
            Rect contentRect = m_ContentRectTrans.rect;
            m_ContentWidth = contentRect.width;
            m_ContentHeight = contentRect.height;

            m_ContentRectTrans.pivot = new Vector2(0f, 1f);
            CheckAnchor(m_ContentRectTrans);

            //设置Content包含Padding的显示位置
            if (m_Direction== e_Direction.Vertical)
                m_ContentRectTrans.anchoredPosition = new Vector2(m_ContentRectTrans.anchoredPosition.x + m_GridContent.padding.left, 0);
            else
                m_ContentRectTrans.anchoredPosition = new Vector2(0, m_ContentRectTrans.anchoredPosition.y + (-1 * m_GridContent.padding.top));

            m_ScrollRect = this.GetComponent<ScrollRect>();

            m_ScrollRect.onValueChanged.RemoveAllListeners();
            //添加滑动事件
            m_ScrollRect.onValueChanged.AddListener(delegate (Vector2 value) { ScrollRectListener(value); });

            if (m_PointingFirstArrow != null || m_PointingEndArrow != null)
            {
                m_ScrollRect.onValueChanged.AddListener(delegate (Vector2 value) { OnDragListener(value); });
                OnDragListener(Vector2.zero);
            }

            //InitScrollBarGameObject(); // 废弃

            m_isInited = true;
        }

        public override void ShowList(int num)
        {
            m_MinIndex = -1;
            m_MaxIndex = -1;

            //-> 计算 Content 尺寸
            if (m_Direction == e_Direction.Vertical)
            {
                //计算每行的高度
                float contentSize = (m_GridContent.spacing.y + m_CellObjectHeight) * Mathf.CeilToInt((float)num / m_Row);
                m_ContentHeight = contentSize;
                m_ContentWidth = m_ContentRectTrans.sizeDelta.x;
                contentSize = contentSize < rectTrans.rect.height ? rectTrans.rect.height : contentSize;
                m_ContentRectTrans.sizeDelta = new Vector2(m_ContentWidth, contentSize);
                if (num != m_MaxCount)
                {
                    m_ContentRectTrans.anchoredPosition = new Vector2(m_ContentRectTrans.anchoredPosition.x, 0);
                }
            }
            else
            {
                float contentSize = (m_GridContent.spacing.x + m_CellObjectWidth) * Mathf.CeilToInt((float)num / m_Row);
                m_ContentWidth = contentSize;
                m_ContentHeight = m_ContentRectTrans.sizeDelta.x;
                contentSize = contentSize < rectTrans.rect.width ? rectTrans.rect.width : contentSize;
                m_ContentRectTrans.sizeDelta = new Vector2(contentSize, m_ContentHeight);
                if (num != m_MaxCount)
                {
                    m_ContentRectTrans.anchoredPosition = new Vector2(0, m_ContentRectTrans.anchoredPosition.y);
                }
            }

            //-> 计算 开始索引
            int lastEndIndex = 0;

            //-> 过多的物体 扔到对象池 ( 首次调 ShowList函数时 则无效 )
            if (m_IsInited)
            {
                lastEndIndex = num - m_MaxCount > 0 ? m_MaxCount : num;
                lastEndIndex = m_IsClearList ? 0 : lastEndIndex;

                int count = m_IsClearList ? m_CellInfos.Length : m_MaxCount;
                for (int i = lastEndIndex; i < count; i++)
                {
                    if (m_CellInfos[i].obj != null)
                    {
                        SetPoolsObj(m_CellInfos[i].obj);
                        m_CellInfos[i].obj = null;
                    }
                }
            }

            //-> 以下四行代码 在for循环所用
            CellInfo[] tempCellInfos = m_CellInfos;
            m_CellInfos = new CellInfo[num];

            GameObject goOnClickNode = null;
            //-> 1: 计算 每个Cell坐标并存储 2: 显示范围内的 Cell
            for (int i = 0; i < num; i++)
            {
                // * -> 存储 已有的数据 ( 首次调 ShowList函数时 则无效 )
                if (m_MaxCount != -1 && i < lastEndIndex)
                {
                    CellInfo tempCellInfo = tempCellInfos[i];
                    //-> 计算是否超出范围
                    float rPos = m_Direction == e_Direction.Vertical ? tempCellInfo.pos.y : tempCellInfo.pos.x;
                    if (!IsOutRange(rPos))
                    {
                        //-> 记录显示范围中的 首位index 和 末尾index
                        m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex; //首位index
                        m_MaxIndex = i; // 末尾index

                        if (tempCellInfo.obj == null)
                        {
                            tempCellInfo.obj = GetPoolsObj();
                        }
                        tempCellInfo.obj.transform.GetComponent<RectTransform>().anchoredPosition = tempCellInfo.pos;
                        tempCellInfo.obj.name = i.ToString();
                        tempCellInfo.obj.SetActive(true);

                        Func(m_FuncCallBackFunc, tempCellInfo.obj);
                    }
                    else
                    {
                        SetPoolsObj(tempCellInfo.obj);
                        tempCellInfo.obj = null;
                    }
                    m_CellInfos[i] = tempCellInfo;
                    continue;
                }

                CellInfo cellInfo = new CellInfo();

                float pos = 0;  //坐标( isVertical ? 记录Y : 记录X )
                float rowPos = 0; //计算每排里面的cell 坐标

                // * -> 计算每个Cell坐标
                if (m_Direction == e_Direction.Vertical)
                {
                    pos = m_CellObjectHeight * Mathf.FloorToInt(i / m_Row) + m_GridContent.spacing.y * Mathf.FloorToInt(i / m_Row);
                    rowPos = m_CellObjectWidth * (i % m_Row) + m_GridContent.spacing.x * (i % m_Row);
                    cellInfo.pos = new Vector3(rowPos + m_GridContent.spacing.x, -pos, 0);
                }
                else
                {
                    pos = m_CellObjectWidth * Mathf.FloorToInt(i / m_Row) + m_GridContent.spacing.y * Mathf.FloorToInt(i / m_Row);
                    rowPos = m_CellObjectHeight * (i % m_Row) + m_GridContent.spacing.x * (i % m_Row);
                    cellInfo.pos = new Vector3(pos, -rowPos, 0);
                }

                //-> 计算是否超出范围
                float cellPos = m_Direction == e_Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (IsOutRange(cellPos))
                {
                    cellInfo.obj = null;
                    m_CellInfos[i] = cellInfo;
                    continue;
                }

                //-> 记录显示范围中的 首位index 和 末尾index
                m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex; //首位index
                m_MaxIndex = i; // 末尾index

                //-> 获取或创建 Cell
                GameObject cell = GetPoolsObj();
                cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                cell.gameObject.name = i.ToString();

                //-> 存数据
                cellInfo.obj = cell;
                m_CellInfos[i] = cellInfo;

                //-> 回调  函数
                Func(m_FuncCallBackFunc, cell);
            }

            m_MaxCount = num;
            m_IsInited = true;

            OnDragListener(Vector2.zero);
        }

        protected override GameObject GetPoolsObj()
        {
            GameObject cell = null;
            if (poolsObj.Count > 0)
            {
                cell = poolsObj.Pop();
            }

            if (cell == null)
            {
                if (m_GoTemplate == null)
                {
                    m_GoTemplate = Instantiate(m_CellGameObject);
                }
                cell = Instantiate(m_GoTemplate) as GameObject;
            }
            cell.transform.SetParent(m_Content.transform,true);
            Vector3 pos = cell.transform.localPosition;
            pos.z = 0;
            cell.transform.localPosition = pos;
            cell.transform.localScale = Vector3.one;
            UIUtils.SetActive(cell, true);

            if (!isAddedListener.ContainsKey(cell))
            {
                GameObject goOnClickNode = FindChildNodeMayBeNull(cell, m_OnClickNodeName);
                if (goOnClickNode != null)
                {
                    Button cellButtonComponent = goOnClickNode.GetComponent<Button>();
                    if (!isAddedListener.ContainsKey(cell) && cellButtonComponent != null)
                    {
                        isAddedListener[cell] = true;
                        cellButtonComponent.onClick.AddListener(delegate () { OnClickCell(cell); });
                    }
                    else
                    {
                        Debug.LogError("=============>> C#层 已经添加了单击事件!! 不再重复添加！");
                    }
                }
                else { Debug.LogError("=============>> C#层 goOnClickNode 按钮的节点为null!! "); }
            }

            return cell;
        }
        public GameObject FindChildNodeMayBeNull(GameObject go, string nodeName)
        {
            Transform tarNode = null;
            GameObject goRelt = null;
            if (!string.IsNullOrEmpty(nodeName))
            {
                tarNode = go.transform.Find(nodeName);
                if (tarNode != null)
                    goRelt = tarNode.gameObject;
            }
            else
            {
                goRelt = null;
            }
            return goRelt;
        }
        protected override void UpdateCheck()
        {
            if (m_CellInfos == null)
                return;

            //检查超出范围
            for (int i = 0, length = m_CellInfos.Length; i < length; i++)
            {
                CellInfo cellInfo = m_CellInfos[i];
                GameObject obj = cellInfo.obj;
                Vector3 pos = cellInfo.pos;

                float rangePos = m_Direction == e_Direction.Vertical ? pos.y : pos.x;
                //判断是否超出显示范围
                if (IsOutRange(rangePos))
                {
                    //把超出范围的cell 扔进 poolsObj里
                    if (obj != null)
                    {
                        SetPoolsObj(obj);
                        m_CellInfos[i].obj = null;
                    }
                }
                else
                {
                    if (obj == null)
                    {
                        GameObject cell = GetPoolsObj();

                        cell.transform.localPosition = pos;
                        cell.gameObject.name = i.ToString();
                        m_CellInfos[i].obj = cell;

                        //Debug.LogError("------->> 未超出范围：" + cell.name);
                        Func(m_FuncCallBackFunc, cell);
                    }
                }
            }
        }
    }
}
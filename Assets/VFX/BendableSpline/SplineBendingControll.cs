using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using UnityEngine.Serialization;



public class SplineBendingControll : MonoBehaviour
{
    [SerializeField] Vector3 _Scale;
    
    [SerializeField] private float _startSpeed = 1f;
    [SerializeField] private float _baseSpeed = 1f;
    [SerializeField] private float _endSpeed = 1f;
    [SerializeField] private  float _nodeInterval = 1f;
    private BendControlMode _bendControlMode;

    [SerializeField] Spline _Spline;
    [SerializeField] ExampleContortAlong _ContortAlong;
    public Vector3 SplineHeadPosition => transform.position + _Spline.nodes[^1].Position;
    public Vector3 SplineHeadDirection => _Spline.nodes[^1].Position - _Spline.nodes[^2].Position;
    
    private Vector3 _startDirection;
    private Vector3 _direction;
    private Transform _headTarget;
    private bool _stopped;
    
    public enum BendControlMode
    {
        Direction,
        Position
    }
    
    public void StartWaterBend(Vector3 dir, BendControlMode _bendControlMode, float speed)
    {
        StartWaterBend(dir, _bendControlMode, speed, speed, speed, new List<Vector3>());
    }
    
    public void StartWaterBend(Vector3 dir, BendControlMode _bendControlMode, float speed, float startSpeed, float endSpeed)
    {
        StartWaterBend(dir,_bendControlMode, speed, startSpeed, endSpeed, new List<Vector3>());
    }
    
    public void StartWaterBend(Vector3 dir, BendControlMode _bendControlMode, float speed, float startSpeed, float endSpeed, List<Vector3> initialNodePoses)
    {
        _startDirection = dir.normalized;
        this._bendControlMode = _bendControlMode;
        _startSpeed = startSpeed;
        _baseSpeed = speed;
        _endSpeed = endSpeed;
        _stopped = false;
        StopAllCoroutines();
        
        StartCoroutine(WaterBend(initialNodePoses));
    }
    
    public void StopWaterBend()
    {
        _stopped = true;
    }

    public void SetDirection(Vector3 dir)
    {
        _direction = dir.normalized;
    }
    
    public void BindHeadTransform(Transform transform)
    {
        _headTarget = transform;
    }

    IEnumerator WaterBend(List<Vector3> initialNodePoses)
    {
        // Remove extra nodes
        List<SplineNode> nodes = new List<SplineNode>(_Spline.nodes);
        for (int i = 2; i < nodes.Count; i++)
        {
            _Spline.RemoveNode(nodes[i]);
        }
        transform.forward = new Vector3(_startDirection.x, 0, _startDirection.z).normalized;
        
        SetInitialNodes(initialNodePoses);
        
        Vector3 startScale = _Scale;
        startScale.x = 0;
        Vector3 targetScale = _Scale;
        
        _ContortAlong.Init(_Scale);
        float meshLength = _ContortAlong.MeshBender.Source.Length;
        meshLength = meshLength == 0 ? 1 : meshLength;
        
        float startingLength = 0;
        float endingLength = 0;
        
        SplineNode lastNode = _Spline.nodes[1];
        var targetStartLength = Mathf.Min(meshLength, _Spline.Length);

        // Handle appear scaling
        while (startingLength < targetStartLength)
        {
            startingLength += Time.deltaTime * _startSpeed;
            _ContortAlong.ScaleMesh(Vector3.Lerp(startScale, targetScale, startingLength / meshLength));
            yield return null;
        }
        //lastNode = AddNode(lastNode);
        // Main bend control
        while (endingLength < meshLength)
        {
            switch (_bendControlMode)
            {
                case BendControlMode.Direction:
                    lastNode = DirectionMove(lastNode);
                    break;
                case BendControlMode.Position:
                    lastNode = PositionMove(lastNode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (_Spline.Length < meshLength) // Handle to short spline
            {
                _ContortAlong.ScaleMesh(Vector3.Lerp(startScale, targetScale, (_Spline.Length / meshLength)));
            }
            else if (_stopped) // Handle disappear
            {
                endingLength += Time.deltaTime * _endSpeed;
                _ContortAlong.ScaleMesh(Vector3.Lerp(targetScale, Vector3.zero, endingLength / meshLength));
                var tailPoint = _Spline.Length - _ContortAlong.MeshBender.Source.Length;
                if (tailPoint < _Spline.Length)
                    _ContortAlong.Contort(tailPoint / _Spline.Length);
            }
            else // Normal movement
            {
                _ContortAlong.Contort((_Spline.Length - meshLength) / _Spline.Length);
            }
            
            yield return null;
        }
        
        Debug.Log("stopped");
    }

    private SplineNode PositionMove(SplineNode lastNode)
    {
        // Move last node
        lastNode.Position = _headTarget.position - transform.position;
        lastNode.Direction = lastNode.Position;

        // Add new node if last node is to far
        if (Vector3.Distance(lastNode.Position, _Spline.nodes[^2].Position) > _nodeInterval)
        {
            lastNode = AddNode(lastNode);
        }

        return lastNode;
    }

    private SplineNode DirectionMove(SplineNode lastNode)
    {
        if (!_stopped)
        {
            // Move last node
            lastNode.Position += _direction * (_baseSpeed * Time.deltaTime);
            lastNode.Direction = lastNode.Position;


            // Add new node if last node is to far
            if (Vector3.Distance(lastNode.Position, _Spline.nodes[^2].Position) > _nodeInterval)
            {
                lastNode = AddNode(lastNode);
            }
        }

        return lastNode;
    }

    private SplineNode AddNode(SplineNode lastNode)
    {
        var splineNode = new SplineNode(lastNode.Position, lastNode.Position);
        _Spline.AddNode(splineNode);
        lastNode = splineNode;
        return lastNode;
    }

    private void SetInitialNodes(List<Vector3> initialNodePoses)
    {
        if (initialNodePoses.Count > 1)
        {
            for (int i = 0; i < initialNodePoses.Count; i++)
            {
                if (i > 1)
                {
                    _Spline.AddNode(new SplineNode(initialNodePoses[i], initialNodePoses[i]));
                }
                else
                {
                    _Spline.nodes[i].Position = initialNodePoses[i];
                    _Spline.nodes[i].Direction = initialNodePoses[i];
                }
            }
        }
        else
        {
            
            _Spline.nodes[0].Position = new Vector3();
            _Spline.nodes[0].Direction = new Vector3();
            _Spline.nodes[1].Position = _startDirection * _nodeInterval;
            _Spline.nodes[1].Direction = _startDirection * _nodeInterval;
        }
    }
}

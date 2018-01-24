/*
Navicat MySQL Data Transfer

Source Server         : 10.240.225.136-dev
Source Server Version : 50150
Source Host           : 10.240.225.136:3306
Source Database       : survey

Target Server Type    : MYSQL
Target Server Version : 50150
File Encoding         : 65001

Date: 2018-01-24 16:23:50
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for answer
-- ----------------------------
DROP TABLE IF EXISTS `answer`;
CREATE TABLE `answer` (
  `answer_id` varchar(50) NOT NULL COMMENT '答案ID主键',
  `apaper_id` int(11) NOT NULL COMMENT '答卷表ID',
  `question_id` varchar(50) NOT NULL,
  `objective_answer` int(11) NOT NULL DEFAULT '0' COMMENT '客观题 答案以二进制方式存放（如果 1 标识 选择了 A，11 选择了AB，101 这选择AC）',
  `subjective_answer` varchar(500) DEFAULT NULL COMMENT '主观题答案',
  PRIMARY KEY (`answer_id`),
  KEY `INDEX_APAPER` (`apaper_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='答题表';

-- ----------------------------
-- Records of answer
-- ----------------------------
INSERT INTO `answer` VALUES ('0253098386264795810c5087fbaf5577', '10', 'f3935584917391a2', '6', '');
INSERT INTO `answer` VALUES ('03819feebd684dc58e1f0146b2c899eb', '14', 'e9dcb3514f1f5f6e', '1', '');
INSERT INTO `answer` VALUES ('082f5c1cca2c4caea9aa57fc843ca12e', '12', '35c8027235805c232', '2', '');
INSERT INTO `answer` VALUES ('1f439e7e3c5749a5954c7556d47ea047', '13', '35c8027235805c232', '2', '');
INSERT INTO `answer` VALUES ('27990d2efecd40f19e1234971fe568e5', '14', '', '0', '3');
INSERT INTO `answer` VALUES ('314d3d75be7941b18d7653607d71d88c', '10', '35c8027235805c232', '1', '');
INSERT INTO `answer` VALUES ('3244a9f773d44a2fbb8002bc56a07174', '12', 'f3935584917391a2', '1', '');
INSERT INTO `answer` VALUES ('90f470e1130c4101bb4078fcd5a7bc5b', '11', 'f3935584917391a2', '7', '');
INSERT INTO `answer` VALUES ('aeaadc72f888475b8331c0272f8e64ee', '13', 'f3935584917391a2', '6', '');
INSERT INTO `answer` VALUES ('b1cc81fe30eb4e5ca45b7c87a8c5a05a', '9', '35c8027235805c232', '1', '');
INSERT INTO `answer` VALUES ('bfdb6fe7354147b49f2f0af446150345', '9', 'f3935584917391a2', '3', '');
INSERT INTO `answer` VALUES ('d2da7b1929cb4d81b22da48b6f4fde42', '11', '35c8027235805c232', '4', '');

-- ----------------------------
-- Table structure for apaper
-- ----------------------------
DROP TABLE IF EXISTS `apaper`;
CREATE TABLE `apaper` (
  `paper_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '答卷Id 自增',
  `qpaper_id` int(11) NOT NULL,
  `qpaper_subject` varchar(200) NOT NULL COMMENT '问卷标题-冗余',
  `qpaper_user_id` varchar(50) NOT NULL COMMENT '问卷创建者-冗余',
  `user_id` varchar(50) NOT NULL COMMENT '答题人标识',
  `create_time` datetime NOT NULL COMMENT '答题时间',
  `remark` varchar(1000) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`paper_id`),
  UNIQUE KEY `INDEX_USERID` (`user_id`,`qpaper_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8 COMMENT='答卷表';

-- ----------------------------
-- Records of apaper
-- ----------------------------
INSERT INTO `apaper` VALUES ('9', '3', '无标题问卷', 'admin', '111111', '2018-01-24 15:41:06', '');
INSERT INTO `apaper` VALUES ('10', '3', '无标题问卷', 'admin', '22222', '2018-01-24 15:41:16', '');
INSERT INTO `apaper` VALUES ('11', '3', '无标题问卷', 'admin', '23333', '2018-01-24 15:41:25', '');
INSERT INTO `apaper` VALUES ('12', '3', '无标题问卷', 'admin', '3333', '2018-01-24 16:15:15', '');
INSERT INTO `apaper` VALUES ('13', '3', '无标题问卷', 'admin', '788888', '2018-01-24 16:17:13', '');
INSERT INTO `apaper` VALUES ('14', '7', '无标题问卷', '', '2', '2018-01-24 16:19:22', '');

-- ----------------------------
-- Table structure for qpaper
-- ----------------------------
DROP TABLE IF EXISTS `qpaper`;
CREATE TABLE `qpaper` (
  `qpaper_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '问卷ID-自增',
  `subject` varchar(500) NOT NULL COMMENT '主题',
  `start_time` datetime DEFAULT NULL COMMENT '调查开始时间',
  `end_time` datetime DEFAULT NULL COMMENT '调查截至时间',
  `description` varchar(1000) DEFAULT NULL COMMENT '说明',
  `apaper_count` int(11) NOT NULL DEFAULT '0' COMMENT '答卷数',
  `create_user_id` varchar(50) NOT NULL COMMENT '创建人/修改人',
  `update_time` datetime NOT NULL,
  PRIMARY KEY (`qpaper_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COMMENT='问卷表';

-- ----------------------------
-- Records of qpaper
-- ----------------------------
INSERT INTO `qpaper` VALUES ('1', 'test1', null, null, '啊啊啊啊啊', '0', 'admin', '2018-01-19 13:34:24');
INSERT INTO `qpaper` VALUES ('2', 'test2', null, null, '111111', '0', 'admin', '2018-01-19 13:39:56');
INSERT INTO `qpaper` VALUES ('3', '无标题问卷', null, null, 'dfsafsafa1222', '5', 'admin', '2018-01-23 14:18:31');
INSERT INTO `qpaper` VALUES ('4', '无标题问卷', null, null, '123aaa', '0', 'admin', '2018-01-19 17:53:15');
INSERT INTO `qpaper` VALUES ('6', '无标题问卷', null, null, '121312313', '0', '', '2018-01-19 17:53:09');
INSERT INTO `qpaper` VALUES ('7', '无标题问卷', null, null, '', '1', '', '2018-01-23 18:00:44');

-- ----------------------------
-- Table structure for question
-- ----------------------------
DROP TABLE IF EXISTS `question`;
CREATE TABLE `question` (
  `id` varchar(50) NOT NULL COMMENT '问题表',
  `topic` varchar(500) NOT NULL COMMENT '问题内容',
  `paper_id` int(11) NOT NULL,
  `question_type` tinyint(4) NOT NULL DEFAULT '0' COMMENT '0 单选题  1 多选题  2 问答题',
  `item_detail` varchar(4000) DEFAULT NULL COMMENT '问题详细格式 格式同["选项1",“选项2”,"选项3"]',
  `sequence` int(11) NOT NULL DEFAULT '0' COMMENT '排序号',
  `extend_input` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否有自定义输入框',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='问题表';

-- ----------------------------
-- Records of question
-- ----------------------------
INSERT INTO `question` VALUES ('35c8027235805c232', '无标题问题1', '3', '0', '111\r222\r333', '1', '\0');
INSERT INTO `question` VALUES ('35c8027235805c87', '无标题问题2', '4', '0', '111\r2\r222\r2222', '2', '\0');
INSERT INTO `question` VALUES ('65c1a24f76a9ff0a', '无标题问题1', '4', '0', '111\r222\r3333', '1', '\0');
INSERT INTO `question` VALUES ('b91378e371816602', '无标题问题1', '6', '0', '111\r2222\r33333', '1', '\0');
INSERT INTO `question` VALUES ('e282d036238d79ea', '无标题问题2', '6', '0', '11\r33', '2', '\0');
INSERT INTO `question` VALUES ('e9dcb3514f1f5f6e', '无标题问题1', '7', '0', '1212', '1', '');
INSERT INTO `question` VALUES ('f3935584917391a2', '无标题问题22', '3', '1', '1112\rdsffsdfs\r123df', '2', '\0');

-- ----------------------------
-- Table structure for user_info
-- ----------------------------
DROP TABLE IF EXISTS `user_info`;
CREATE TABLE `user_info` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '用户ID 自增主键',
  `account` varchar(50) NOT NULL COMMENT '用户登录账号',
  `full_name` varchar(50) NOT NULL COMMENT '用户姓名',
  `password` varchar(50) NOT NULL COMMENT '登录密码',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_time` datetime NOT NULL COMMENT '最后更新时间',
  `is_admin` bit(1) NOT NULL DEFAULT b'0' COMMENT '是否超级管理员',
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `account_UNIQUE` (`account`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='用户信息表';

-- ----------------------------
-- Records of user_info
-- ----------------------------
INSERT INTO `user_info` VALUES ('1', 'admin', '假正经哥哥', '000102030405060708090a0b0c0d0e0f', '2018-01-11 16:16:47', '2018-01-11 16:16:47', '\0');
